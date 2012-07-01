using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CGO.Web.Infrastructure;
using Newtonsoft.Json;

namespace CGO.Web.Controllers
{
    public class GoogleController : Controller
    {
        private const string PostDataSeparator = "&";
        private const string PostDataPairSeparator = "=";
        private const string OAuthRequestContentType = "application/x-www-form-urlencoded";
        private const string OAuthRequestMethod = "POST";
        private readonly IFormsAuthenticationService formsAuthenticationService;
        private readonly IOAuthConfiguration oAuthConfiguration;

        public GoogleController(IFormsAuthenticationService formsAuthenticationService, IOAuthConfiguration oAuthConfiguration)
        {
            if (formsAuthenticationService == null)
            {
                throw new ArgumentNullException("formsAuthenticationService");
            }

            if (oAuthConfiguration == null)
            {
                throw new ArgumentNullException("oAuthConfiguration");
            }

            this.formsAuthenticationService = formsAuthenticationService;
            this.oAuthConfiguration = oAuthConfiguration;
        }

        public ActionResult OAuthCallback(string code, string state, string redirectUrl = null)
        {
            var json = ConvertAuthCodeToJson(code);
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            var userInfoUrl = string.Format(oAuthConfiguration.UserProfileUrl, data.access_token);

            var userInfoJson = new WebClient().DownloadString(userInfoUrl);
            var userInfo = JsonConvert.DeserializeObject<dynamic>(userInfoJson);
            var username = (string) userInfo.name;
            formsAuthenticationService.SetAuthenticationCookie(username, false);

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return Redirect(redirectUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private string ConvertAuthCodeToJson(string authCode)
        {
            var postData = new Dictionary<string, string>
            {
                {"code", authCode}, 
                {"client_id", oAuthConfiguration.ClientId},
                {"client_secret", oAuthConfiguration.ClientSecret},
                {"redirect_uri", oAuthConfiguration.OauthCallbackUrl},
                {"grant_type", oAuthConfiguration.OauthGrantType}
            };

            return GetHttpPostResponse(oAuthConfiguration.OauthTokenUrl, postData);
        }

        private string GetHttpPostResponse(string url, Dictionary<string, string> postData)
        {
            var postString = string.Join(PostDataSeparator, postData.Select(
                pair => HttpUtility.UrlEncode(pair.Key) + PostDataPairSeparator + HttpUtility.UrlEncode(pair.Value)
            )).ToArray();

            var postBytes = Encoding.ASCII.GetBytes(postString);
            var request = HttpWebRequest.Create(url);
            request.Method = OAuthRequestMethod;
            request.ContentType = OAuthRequestContentType;
            request.ContentLength = postBytes.Length;
            
            var postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            var response = request.GetResponse() as HttpWebResponse;
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return json;
        }
    }
}
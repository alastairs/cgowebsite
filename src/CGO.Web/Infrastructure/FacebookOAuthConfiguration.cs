using System;
using System.Configuration;
using System.Web;

namespace CGO.Web.Infrastructure
{
    public class FacebookOAuthConfiguration : IOAuthConfiguration
    {
        private const string Scope = "";
        private const string Provider = "facebook";

        public string OauthTokenUrl { get { return ConfigurationManager.AppSettings["facebook_oauth_token_url"]; } }
        public string OauthCallbackUrl { get { return string.Format(ConfigurationManager.AppSettings["oauth_redirect_url"], Provider); } }
        public string ClientId { get { return ConfigurationManager.AppSettings["facebook_client_id"]; } }
        public string ClientSecret { get { return ConfigurationManager.AppSettings["facebook_client_secret"]; } }
        public string OauthGrantType { get { return ConfigurationManager.AppSettings["oauth_grant_type"]; } }
        public string UserProfileUrl { get { return ConfigurationManager.AppSettings["facebook_user_profile_url"]; } }

        public string MakeLoginUri(string state)
        {
            return string.Format(ConfigurationManager.AppSettings["facebook_oauth_login_url"],
                                 HttpUtility.UrlEncode(Scope), HttpUtility.UrlEncode(state),
                                 HttpUtility.UrlEncode(OauthCallbackUrl), HttpUtility.UrlEncode(ClientId));
        }
    }
}
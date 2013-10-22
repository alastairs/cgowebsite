using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

using CGO.Web.Infrastructure;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace CGO.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IFormsAuthenticationService formsAuthentication;

        public UserController(IFormsAuthenticationService formsAuthentication)
        {
            if (formsAuthentication == null)
            {
                throw new ArgumentNullException("formsAuthentication");
            }

            this.formsAuthentication = formsAuthentication;
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        public ActionResult Logout()
        {
            formsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Authenticate(string returnUrl)
        {
            var openId = new OpenIdRelyingParty();

            var openIdResponse = await openId.GetResponseAsync();
            if (openIdResponse == null)
            {
                return await SendRequestToProvider(openId);
            }
            
            return ProcessResponseFromProvider(returnUrl, openIdResponse);
        }

        private async Task<ActionResult> SendRequestToProvider(OpenIdRelyingParty openId)
        {
            var submittedId = !string.IsNullOrWhiteSpace(Request.Form["openid_manual"]) ? Request.Form["openid_manual"] : Request.Form["openid_identifier"];

            Identifier id;
            if (Identifier.TryParse(submittedId, out id))
            {
                try
                {
                    var fetchRequest = new FetchRequest();

                    fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                    fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.FullName);
                    fetchRequest.Attributes.AddOptional(WellKnownAttributes.Name.Alias);

                    var authenticationRequest = await openId.CreateRequestAsync(id);
                    authenticationRequest.AddExtension(fetchRequest);

                    var authenticationResponse = await authenticationRequest.GetRedirectingResponseAsync();
                    return authenticationResponse.AsActionResult();
                }
                catch (ProtocolException ex)
                {
                    return HandleOpenIdError(ex.Message);
                }
            }

            return HandleOpenIdError("Invalid Identifier");
        }

        private ActionResult ProcessResponseFromProvider(string returnUrl, IAuthenticationResponse openIdResponse)
        {
            switch (openIdResponse.Status)
            {
                case AuthenticationStatus.Authenticated:
                    return ProcessSuccessfulResponseFromProvider(returnUrl, openIdResponse);
                case AuthenticationStatus.Canceled:
                    return HandleOpenIdError("Cancelled at provider");
                case AuthenticationStatus.Failed:
                    return HandleOpenIdError(openIdResponse.Exception.Message);
            }

            return new EmptyResult();
        }

        private ActionResult ProcessSuccessfulResponseFromProvider(string returnUrl, IAuthenticationResponse openIdResponse)
        {
            var fetchResponse = openIdResponse.GetExtension<FetchResponse>();

            string email = null, fullName = null, alias = null;
            if (fetchResponse != null)
            {
                email = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Email);
                fullName = fetchResponse.GetAttributeValue(WellKnownAttributes.Name.FullName);
                alias = fetchResponse.GetAttributeValue(WellKnownAttributes.Name.Alias);
            }

            Session["DisplayName"] = alias ?? fullName ?? email ?? openIdResponse.FriendlyIdentifierForDisplay;
            FormsAuthentication.SetAuthCookie(openIdResponse.FriendlyIdentifierForDisplay, false);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private ActionResult HandleOpenIdError(string message)
        {
            ViewBag.Message = message;
            return View("Login");
        }
    }
}

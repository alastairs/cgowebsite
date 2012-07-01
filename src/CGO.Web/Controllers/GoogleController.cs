using System;
using System.Web.Mvc;
using CGO.Web.Infrastructure;

namespace CGO.Web.Controllers
{
    public class GoogleController : Controller
    {
        private readonly IFormsAuthenticationService formsAuthenticationService;

        public GoogleController(IFormsAuthenticationService formsAuthenticationService)
        {
            if (formsAuthenticationService == null)
            {
                throw new ArgumentNullException("formsAuthenticationService");
            }

            this.formsAuthenticationService = formsAuthenticationService;
        }

        public ActionResult OAuthCallback(string code, string state, string redirectUrl = null)
        {
            formsAuthenticationService.SetAuthenticationCookie("Bob", false);

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return Redirect(redirectUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
using System;
using System.Web.Security;
using CGO.Web.Controllers;
using CGO.Web.Infrastructure;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

namespace CGO.Web.Tests
{
    public class GoogleControllerFacts
    {
        [TestFixture]
        public class OAuthCallbackShould
        {
            [Test]
            public void RedirectToHomepageWhenNoRedirectUrlIsSet()
            {
                var controller = new GoogleController(Substitute.For<IFormsAuthenticationService>());

                var result = controller.OAuthCallback("code", "state");

                result.AssertActionRedirect().ToAction<HomeController>(c => c.Index());
            }

            [Test]
            public void RedirectToGivenUrlWhenARedirectUrlIsSet()
            {
                var controller = new GoogleController(Substitute.For<IFormsAuthenticationService>());

                const string redirectUrl = "/Concerts";
                var result = controller.OAuthCallback("code", "state", redirectUrl);

                result.AssertHttpRedirect().ToUrl(redirectUrl);
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheProvidedFormsAuthenticationServiceIsNull()
            {
                Assert.That(() => new GoogleController(null), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void SetTheFormsAuthenticationCookieOnSuccessfulLogin()
            {
                var formsAuthenticationService = Substitute.For<IFormsAuthenticationService>();
                var controller = new GoogleController(formsAuthenticationService);

                controller.OAuthCallback("code", "state");
                
                formsAuthenticationService.Received().SetAuthenticationCookie("Bob", false);
            }
        }
    }
}

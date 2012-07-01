﻿using System;
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
                var oAuthConfiguration = Substitute.For<IOAuthConfiguration>();
                oAuthConfiguration.OauthTokenUrl.Returns("http://localhost/");
                var controller = new GoogleController(Substitute.For<IFormsAuthenticationService>(), oAuthConfiguration);

                var result = controller.OAuthCallback("code", "state");

                result.AssertActionRedirect().ToAction<HomeController>(c => c.Index());
            }

            [Test]
            public void RedirectToGivenUrlWhenARedirectUrlIsSet()
            {
                var oAuthConfiguration = Substitute.For<IOAuthConfiguration>();
                oAuthConfiguration.OauthTokenUrl.Returns("http://localhost/");
                var controller = new GoogleController(Substitute.For<IFormsAuthenticationService>(), oAuthConfiguration);

                const string redirectUrl = "/Concerts";
                var result = controller.OAuthCallback("code", "state", redirectUrl);

                result.AssertHttpRedirect().ToUrl(redirectUrl);
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheProvidedFormsAuthenticationServiceIsNull()
            {
                Assert.That(() => new GoogleController(null, Substitute.For<IOAuthConfiguration>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheProvidedOAuthConfigurationIsNull()
            {
                Assert.That(() => new GoogleController(Substitute.For<IFormsAuthenticationService>(), null), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void SetANonPersistentFormsAuthenticationCookieOnSuccessfulLogin()
            {
                var formsAuthenticationService = Substitute.For<IFormsAuthenticationService>();
                var oAuthConfiguration = Substitute.For<IOAuthConfiguration>();
                oAuthConfiguration.OauthTokenUrl.Returns("http://localhost/");
                var controller = new GoogleController(formsAuthenticationService, oAuthConfiguration);

                controller.OAuthCallback("code", "state");
                
                formsAuthenticationService.Received().SetAuthenticationCookie("Bob", false);
            }
        }
    }
}

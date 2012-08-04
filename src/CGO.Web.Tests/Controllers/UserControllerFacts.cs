using System;

using CGO.Web.Controllers;
using CGO.Web.Infrastructure;

using NSubstitute;

using NUnit.Framework;

using MvcContrib.TestHelper;

namespace CGO.Web.Tests.Controllers
{
    class UserControllerFacts
    {
        [TestFixture]
        public class ConstructorShould
        {
            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheFormsAuthenticationServiceIsNull()
            {
                Assert.That(() => new UserController(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class LoginShould
        {
            [Test]
            public void DisplayTheLoginView()
            {
                var controller = new UserController(Substitute.For<IFormsAuthenticationService>());

                var result = controller.Login();

                result.AssertViewRendered().ForView("Login");
            }
        }

        [TestFixture]
        public class LogoutShould
        {
            [Test]
            public void ClearTheAuthenticationCookie()
            {
                var formsAuthentication = Substitute.For<IFormsAuthenticationService>();
                var controller = new UserController(formsAuthentication);

                controller.Logout();

                formsAuthentication.Received().SignOut();
            }

            [Test]
            public void ReturnARedirectResult()
            {
                var controller = new UserController(Substitute.For<IFormsAuthenticationService>());

                var result = controller.Logout();

                result.AssertActionRedirect();
            }

            [Test]
            public void RedirectToTheHomePage()
            {
                var controller = new UserController(Substitute.For<IFormsAuthenticationService>());

                var result = controller.Logout();

                result.AssertActionRedirect().ToAction((HomeController h) => h.Index());
            }
        }
    }
}

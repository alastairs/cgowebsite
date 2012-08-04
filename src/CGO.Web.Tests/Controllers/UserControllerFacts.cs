using CGO.Web.Controllers;

using NUnit.Framework;

using MvcContrib.TestHelper;

namespace CGO.Web.Tests.Controllers
{
    class UserControllerFacts
    {
        [TestFixture]
        public class LoginShould
        {
            [Test]
            public void DisplayTheLoginView()
            {
                var controller = new UserController();

                var result = controller.Login();

                result.AssertViewRendered().ForView("Login");
            }
        }
    }
}

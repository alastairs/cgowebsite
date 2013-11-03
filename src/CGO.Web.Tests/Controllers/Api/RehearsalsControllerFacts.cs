using System.Net;

using CGO.Web.Areas.Admin.Controllers.Api;
using CGO.Web.Areas.Admin.Models.Api;

using NUnit.Framework;

namespace CGO.Web.Tests.Controllers.Api
{
    public class RehearsalsControllerFacts
    {
        [TestFixture]
        public class PostShould
        {
            [Test]
            public void IndicateTheRehearsalWasCreated()
            {
                var controller = new RehearsalsController();

                var result = controller.Post(new RehearsalApiModel
                {
                    Id = 0
                });

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }
        }
    }
}

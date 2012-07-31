using System;

using CGO.Web.Controllers.Api;
using CGO.Web.Models;

using NUnit.Framework;

namespace CGO.Web.Tests.Controllers.Api
{
    public class ConcertsApiControllerFacts
    {
        [TestFixture]
        public class DeleteShould : RavenTest
        {
            private int idToDelete;

            [Test]
            public void RemoveTheConcertWithTheSpecifiedIdFromTheDatabase()
            {
                var controller = new ConcertsApiController(Session);

                controller.Delete(idToDelete);

                Assert.That(Session.Load<Concert>(idToDelete), Is.Null);
            }

            [SetUp]
            public void CreateSampleData()
            {
                using (var sampleDataSession = Store.OpenSession())
                {
                    idToDelete = 1;
                    sampleDataSession.Store(new Concert(idToDelete, "Foo", DateTime.Now, "Bar"));
                    sampleDataSession.SaveChanges();
                }
            }
        }
    }
}

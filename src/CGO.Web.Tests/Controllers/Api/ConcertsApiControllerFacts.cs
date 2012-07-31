using System;

using CGO.Web.Controllers.Api;
using CGO.Web.Models;

using NSubstitute;

using NUnit.Framework;

using Raven.Client;

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

            [Test]
            public void DoNothingIfTheConcertToDeleteDoesNotExist()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = new ConcertsApiController(mockRavenSession);

                var nonExistentDocument = idToDelete + 1;
                controller.Delete(nonExistentDocument);

                mockRavenSession.DidNotReceive().Delete(Arg.Any<Concert>());
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

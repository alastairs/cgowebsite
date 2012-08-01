using System;
using System.Net;

using CGO.Web.Controllers.Api;
using CGO.Web.Models;

using NSubstitute;

using NUnit.Framework;

using Raven.Client;

namespace CGO.Web.Tests.Controllers.Api
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class DeleteShould : RavenTest
        {
            private int idToDelete;

            [Test]
            public void RemoveTheConcertWithTheSpecifiedIdFromTheDatabase()
            {
                var controller = new ConcertsController(Session);

                controller.DeleteConcert(idToDelete);

                Assert.That(Session.Load<Concert>(idToDelete), Is.Null);
            }

            [Test]
            public void DoNothingIfTheConcertToDeleteDoesNotExist()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = new ConcertsController(mockRavenSession);

                var nonExistentDocument = idToDelete + 1;
                controller.DeleteConcert(nonExistentDocument);

                mockRavenSession.DidNotReceive().Delete(Arg.Any<Concert>());
            }

            [Test]
            public void ReturnA204NoContentResponseCode()
            {
                var controller = new ConcertsController(Session);

                var result = controller.DeleteConcert(idToDelete);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
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

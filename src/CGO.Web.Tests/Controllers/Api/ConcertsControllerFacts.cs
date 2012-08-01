using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using CGO.Web.Controllers.Api;
using CGO.Web.Models;
using CGO.Web.Tests.EqualityComparers;

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

        [TestFixture]
        public class GetShould : RavenTest
        {
            private IEnumerable<Concert> concerts;

            [Test]
            public void ReturnAnEnumerableOfConcerts()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Get();

                Assert.That(result, Is.InstanceOf<IEnumerable<Concert>>());
            }

            [Test]
            public void ReturnAllTheConcertsInTheDatabase()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Get();

                Assert.That(result, Is.EquivalentTo(concerts).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void ReturnTheConcertsInDescendingOrderOfDate()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Get();

                Assert.That(result, Is.EqualTo(concerts.OrderByDescending(c => c.DateAndStartTime)).Using(new ConcertEqualityComparer()));
            }

            [SetUp]
            public void CreateSampleData()
            {
                concerts = new[]
                {
                    new Concert(1, "Foo", new DateTime(2012, 08, 01, 20, 00, 00), "Bar"),
                    new Concert(2, "Foo", new DateTime(2012, 08, 02, 20, 00, 00), "Bar"),
                    new Concert(3, "Foo", new DateTime(2012, 07, 31, 20, 00, 00), "Bar")
                };

                using (var sampleDataSession = Store.OpenSession())
                {
                    foreach (var concert in concerts)
                    {
                        sampleDataSession.Store(concert);
                    }
                    
                    sampleDataSession.SaveChanges();
                }
            }
        }
    }
}

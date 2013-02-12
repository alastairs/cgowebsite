using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using CGO.Domain;
using CGO.Web.Controllers.Api;
using CGO.Web.Mappers;
using CGO.Web.Tests.EqualityComparers;
using CGO.Web.ViewModels.Api;

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
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                controller.DeleteConcert(idToDelete);

                Assert.That(Session.Load<Concert>(idToDelete), Is.Null);
            }

            [Test]
            public void DoNothingIfTheConcertToDeleteDoesNotExist()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = new ConcertsController(mockRavenSession, Substitute.For<IConcertDetailsService>());

                var nonExistentDocument = idToDelete + 1;
                controller.DeleteConcert(nonExistentDocument);

                mockRavenSession.DidNotReceive().Delete(Arg.Any<Concert>());
            }

            [Test]
            public void ReturnA204NoContentResponseCode()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

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
            private IEnumerable<ConcertViewModel> viewModels;

            [Test]
            public void ReturnAnEnumerableOfConcertViewModels()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Get();

                Assert.That(result, Is.InstanceOf<IEnumerable<ConcertViewModel>>());
            }

            [Test]
            public void ReturnAllTheConcertsInTheDatabase()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Get();

                Assert.That(result, Is.EquivalentTo(viewModels).Using(new ConcertApiViewModelEqualityComparer()));
            }

            [Test]
            public void ReturnTheConcertsInDescendingOrderOfDate()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Get();

                Assert.That(result, Is.EqualTo(viewModels.OrderByDescending(c => c.DateAndStartTime)).Using(new ConcertApiViewModelEqualityComparer()));
            }

            [Test]
            public void SetTheHrefPropertyOnTheViewModel()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var concerts = controller.Get().ToList();

                Assert.That(concerts.Select(c => c.Href), Is.EqualTo(concerts.Select(c => "/api/concerts/" + c.Id)));
            }

            [SetUp]
            public void CreateSampleData()
            {
                viewModels = new[]
                {
                    new ConcertViewModel
                        {
                                    Id = 1,
                                    Title = "Foo",
                                    DateAndStartTime = new DateTime(2012, 08, 01, 20, 00, 00), 
                                    Location = "Bar"
                        },
                    new ConcertViewModel
                        {
                                    Id = 2, 
                                    Title = "Foo", 
                                    DateAndStartTime = new DateTime(2012, 08, 02, 20, 00, 00), 
                                    Location = "Bar",
                        },
                    new ConcertViewModel
                        {
                                    Id = 3, 
                                    Title = "Foo", 
                                    DateAndStartTime = new DateTime(2012, 07, 31, 20, 00, 00), 
                                    Location = "Bar"
                        }
                };

                using (var sampleDataSession = Store.OpenSession())
                {
                    foreach (var viewModel in viewModels)
                    {
                        sampleDataSession.Store(viewModel.ToModel<Concert, ConcertViewModel>());
                    }
                    
                    sampleDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class PostShould
        {
            private readonly ConcertViewModel concertRequest = new ConcertViewModel
            {
                Id = 0, 
                Title = "Foo", 
                DateAndStartTime = new DateTime(2012, 08, 01, 20, 00, 00), 
                Location = "Bar"
            };

            private HttpRequestMessage request;
            private HttpControllerContext controllerContext;

            [TestFixtureSetUp]
            public void ConfigureWebApi()
            {
                var config = new HttpConfiguration();

                request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/concerts");
                request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

                var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
                var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "concerts" } });
                
                controllerContext = new HttpControllerContext(config, routeData, request);
            }

            [Test]
            public void ReturnA201CreatedStatusCodeIfTheConcertModelIsOk()
            {
                var controller = CreateConcertsController(Substitute.For<IDocumentSession>());
                var result = controller.Post(concertRequest);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }

            [Test]
            public void ReturnTheNewConcertsHrefInTheLocationHeader()
            {
                var controller = CreateConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Post(concertRequest);

                Assert.That(result.Headers.Location, Is.Not.Null);
            }

            [Test]
            public async void ReturnTheNewConcertInTheResponseBody()
            {
                var controller = CreateConcertsController(Substitute.For<IDocumentSession>());
                var expected = concertRequest;
                expected.Id = 0;
                expected.Href = "/api/concerts/0";

                var result = await controller.Post(concertRequest).Content.ReadAsAsync<ConcertViewModel>();

                Assert.That(result, Is.EqualTo(expected).Using(new ConcertApiViewModelEqualityComparer()));
            }

            [Test]
            public void CallStoreOnTheRavenSessionIfTheConcertModelIsOk()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = CreateConcertsController(mockRavenSession);

                controller.Post(concertRequest);

                var concertToSave = new Concert(concertRequest.Id, concertRequest.Title, concertRequest.DateAndStartTime, concertRequest.Location);
                mockRavenSession.Received().Store(Arg.Is<Concert>(x => new ConcertEqualityComparer().Equals(x, concertToSave)));
            }

            [Test]
            public void CallSaveChangesOnTheRavenSessionIfTheConcertModelIsOk()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = CreateConcertsController(mockRavenSession);

                controller.Post(concertRequest);
                

                mockRavenSession.Received().SaveChanges();
            }

            [Test]
            public void ReturnA400BadRequestIfTheConcertModelIsNull()
            {
                var controller = CreateConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Post(null);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            private ConcertsController CreateConcertsController(IDocumentSession documentSession)
            {
                return new ConcertsController(documentSession, Substitute.For<IConcertDetailsService>())
                {
                    ControllerContext = controllerContext,
                    Request = request
                };
            }
        }

        [TestFixture]
        public class PutShould : RavenTest
        {
            private IEnumerable<ConcertViewModel> viewModels; 
                
            [Test]
            public void ReturnA400BadRequestIfTheConcertModelIsNull()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Put(4, null);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [TestCase(0)]
            [TestCase(-1)]
            [TestCase(4)]
            public void ReturnA404NotFoundIfTheSpecifiedIdIsUnknown(int concertId)
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Put(concertId, viewModels.First());

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }

            [Test]
            public void ReturnA204NoContentWhenTheEditSucceeds()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var concertToEdit = viewModels.First();
                concertToEdit.IsPublished = true;
                var result = controller.Put(1, concertToEdit);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            }

            [Test]
            public void PersistChangesIfTheConcertViewModelIsOk()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                var concertToEdit = viewModels.First();
                concertToEdit.Title = "Bar";

                controller.Put(1, concertToEdit);

                Assert.That(Session.Load<Concert>(1).Title, Is.EqualTo("Bar"));
            }

            [SetUp]
            public void CreateSampleData()
            {
                viewModels = new[]
                {
                    new ConcertViewModel
                        {
                                    Id = 1,
                                    Title = "Foo",
                                    DateAndStartTime = new DateTime(2012, 08, 01, 20, 00, 00), 
                                    Location = "Bar"
                        },
                    new ConcertViewModel
                        {
                                    Id = 2, 
                                    Title = "Foo", 
                                    DateAndStartTime = new DateTime(2012, 08, 02, 20, 00, 00), 
                                    Location = "Bar",
                        },
                    new ConcertViewModel
                        {
                                    Id = 3, 
                                    Title = "Foo", 
                                    DateAndStartTime = new DateTime(2012, 07, 31, 20, 00, 00), 
                                    Location = "Bar"
                        }
                };

                using (var sampleDataSession = Store.OpenSession())
                {
                    foreach (var viewModel in viewModels)
                    {
                        sampleDataSession.Store(viewModel.ToModel<Concert, ConcertViewModel>());
                    }

                    sampleDataSession.SaveChanges();
                }
            }
        }
    }
}

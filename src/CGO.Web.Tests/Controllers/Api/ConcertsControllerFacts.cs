using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using CGO.Web.Controllers.Api;
using CGO.Web.Mappers;
using CGO.Web.Models;
using CGO.Web.Tests.EqualityComparers;
using CGO.Web.ViewModels;

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
            private IEnumerable<ConcertViewModel> viewModels;

            [Test]
            public void ReturnAnEnumerableOfConcertViewModels()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Get();

                Assert.That(result, Is.InstanceOf<IEnumerable<ConcertViewModel>>());
            }

            [Test]
            public void ReturnAllTheConcertsInTheDatabase()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Get();

                Assert.That(result, Is.EquivalentTo(viewModels).Using(new ConcertViewModelEqualityComparer()));
            }

            [Test]
            public void ReturnTheConcertsInDescendingOrderOfDate()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Get();

                Assert.That(result, Is.EqualTo(viewModels.OrderByDescending(c => c.Date)).Using(new ConcertViewModelEqualityComparer()));
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
                                    Date = new DateTime(2012, 08, 01, 20, 00, 00), 
                                    StartTime = new DateTime(2012, 08, 01, 20, 00, 00), 
                                    Location = "Bar"
                        },
                    new ConcertViewModel
                        {
                                    Id = 2, 
                                    Title = "Foo", 
                                    Date = new DateTime(2012, 08, 02, 20, 00, 00), 
                                    StartTime = new DateTime(2012, 08, 02, 20, 00, 00), 
                                    Location = "Bar",
                        },
                    new ConcertViewModel
                        {
                                    Id = 3, 
                                    Title = "Foo", 
                                    Date = new DateTime(2012, 07, 31, 20, 00, 00), 
                                    StartTime = new DateTime(2012, 07, 31, 20, 00, 00), 
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
                Date = new DateTime(2012, 08, 01, 20, 00, 00), 
                StartTime = new DateTime(2012, 08, 01, 20, 00, 00), 
                Location = "Bar"
            };

            [Test]
            public void ReturnA201CreatedStatusCodeIfTheConcertModelIsOk()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Post(concertRequest);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }

            [Test]
            public void CallStoreOnTheRavenSessionIfTheConcertModelIsOk()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = new ConcertsController(mockRavenSession);

                controller.Post(concertRequest);

                var concertToSave = new Concert(concertRequest.Id, concertRequest.Title, concertRequest.Date, concertRequest.Location);
                mockRavenSession.Received().Store(Arg.Is<Concert>(x => new ConcertEqualityComparer().Equals(x, concertToSave)));
            }

            [Test]
            public void CallSaveChangesOnTheRavenSessionIfTheConcertModelIsOk()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = new ConcertsController(mockRavenSession);

                controller.Post(concertRequest);

                mockRavenSession.Received().SaveChanges();
            }

            [Test]
            public void ReturnA400BadRequestIfTheConcertModelIsNull()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Post(null);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
        }

        [TestFixture]
        public class PutShould : RavenTest
        {
            private IEnumerable<ConcertViewModel> viewModels; 
                
            [Test]
            public void ReturnA400BadRequestIfTheConcertModelIsNull()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Put(4, null);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }

            [Test]
            public void ReturnA204NoContentWhenTheEditSucceeds()
            {
                var controller = new ConcertsController(Session);

                var concertToEdit = viewModels.First();
                concertToEdit.IsPublished = true;
                var result = controller.Put(1, concertToEdit);

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
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
                                    Date = new DateTime(2012, 08, 01, 20, 00, 00), 
                                    StartTime = new DateTime(2012, 08, 01, 20, 00, 00), 
                                    Location = "Bar"
                        },
                    new ConcertViewModel
                        {
                                    Id = 2, 
                                    Title = "Foo", 
                                    Date = new DateTime(2012, 08, 02, 20, 00, 00), 
                                    StartTime = new DateTime(2012, 08, 02, 20, 00, 00), 
                                    Location = "Bar",
                        },
                    new ConcertViewModel
                        {
                                    Id = 3, 
                                    Title = "Foo", 
                                    Date = new DateTime(2012, 07, 31, 20, 00, 00), 
                                    StartTime = new DateTime(2012, 07, 31, 20, 00, 00), 
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

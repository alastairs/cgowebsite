using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Domain;
using CGO.Web.Controllers;
using CGO.Web.Mappers;
using CGO.Web.Tests.EqualityComparers;
using CGO.Web.ViewModels;

using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

using Raven.Client;
using Rhino.Mocks;

namespace CGO.Web.Tests.Controllers
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class WhenThereAreConcerts_IndexShould : RavenTest
        {
            private IEnumerable<Concert> concerts;

            [Test]
            public void DisplayTheIndexView()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsInAscendingOrderByDate()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Index() as ViewResult;
                var concertsDisplayed = result.Model as IEnumerable<Concert>;

                Assert.That(concertsDisplayed, Is.EqualTo(concerts.OrderBy(c => c.DateAndStartTime)).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void DisplayTheConcertsFromTheDatabase()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Index() as ViewResult;

                Assert.That(result.Model, Is.EquivalentTo(concerts).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void DisplayOnlyConcertsInTheFuture()
            {
                var pastConcert = new Concert(5, "Concert in the past", new DateTime(2011, 11, 18, 20, 00, 00), "West Road Concert Hall");
                Session.Store(pastConcert);
                Session.SaveChanges();
                
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                
                var result = controller.Index() as ViewResult;

                Assert.That(result.Model, Is.Not.Contains(pastConcert).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void DisplayOnlyPublishedConcerts()
            {
                var unpublishedConcert = new Concert(5, "Unpublished concert", new DateTime(2101, 01, 18, 20, 00, 00), "West Road Concert Hall");
                Session.Store(unpublishedConcert);
                Session.SaveChanges();

                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Index() as ViewResult;

                Assert.That(result.Model, Is.Not.Contains(unpublishedConcert).Using(new ConcertEqualityComparer()));
            }

            [SetUp]
            public void CreateSampleData()
            {
                concerts = new List<Concert>
                {
                    new Concert(1, "2100 Concert 1", new DateTime(2100, 01, 18, 20, 00, 00), "West Road Concert Hall"),
                    new Concert(2, "2100 Concert 2", new DateTime(2100, 02, 18, 20, 00, 00), "West Road Concert Hall"),
                    new Concert(3, "2100 Concert 3", new DateTime(2100, 03, 18, 20, 00, 00), "West Road Concert Hall"),
                    new Concert(4, "2100 Concert 4", new DateTime(2100, 04, 18, 20, 00, 00), "West Road Concert Hall")
                };

                foreach (var concert in concerts)
                {
                    concert.Publish();
                }

                using (var sampleDataSession = Store.OpenSession())
                {
                    foreach(var concert in concerts)
                    {
                        sampleDataSession.Store(concert);
                    }

                    sampleDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class WhenThereAreNoConcerts_IndexShould : RavenTest
        {
            [Test]
            public void DisplayTheSiteHomePageIfTheRequestIsAnonymous()
            {
                var builder = new TestControllerBuilder();
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                builder.InitializeController(controller);

                var result = controller.Index();

                result.AssertActionRedirect().ToAction("Index").ToController("Home");
            }

            [Test]
            public void DisplayTheCreateViewIfTheRequestIsAuthenticated()
            {
                var builder = new TestControllerBuilder();
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                builder.InitializeController(controller);
                controller.Request.Stub(r => r.IsAuthenticated).Return(true); // Have to use RhinoMocks here, as that's what MvcContrib uses
                
                var result = controller.Index();

                result.AssertViewRendered().ForView("List");
            }
        }

        [TestFixture]
        public class DetailsShould
        {
            private readonly Concert sampleConcert = new Concert(1, "Test Concert", DateTime.MinValue, "Venue");

            [Test]
            public void RenderTheDetailsView()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetConcert(1).ReturnsForAnyArgs(sampleConcert);
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), concertDetailsService);

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                const int requestedConcertId = 1;
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetConcert(requestedConcertId).Returns(sampleConcert);
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), concertDetailsService);

                var result = controller.Details(requestedConcertId) as ViewResult;
                var concert = result.Model as Concert;
                
                Assert.That(concert.Id, Is.EqualTo(requestedConcertId));
            }

            [Test]
            public void ReturnA404NotFoundWhenTheConcertDoesntExist()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Details(2);

                result.AssertResultIs<HttpNotFoundResult>();
            }
        }

        [TestFixture]
        public class CreateShould : RavenTest
        {
            [Test]
            public void ShowTheCreateViewWhenCalledViaAGetRequest()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Create(); // The parameterless overload is called on GET.

                result.AssertViewRendered().ForView("Create");
            }

            [Test]
            public void ShowTheCreateViewWithTheSuppliedModelWhenThereAreValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());
                controller.ViewData.ModelState.AddModelError("Title", "Please enter a title");

                var result = controller.Create(new ConcertViewModel()); // The overload with ConcertViewModel parameter is called on POST.

                result.AssertViewRendered().ForView("Create").WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ReturnToTheListOfConcertsWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Create(new ConcertViewModel());

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void StoreTheConcertInTheDatabaseWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                var concert = new Concert(1, "Foo", new DateTime(2012, 07, 29, 20, 00, 00), "Bar");

                controller.Create(new ConcertViewModel
                {
                    Date = concert.DateAndStartTime,
                    StartTime = concert.DateAndStartTime,
                    Location = concert.Location,
                    Title = concert.Title
                });

                Assert.That(Session.Load<Concert>(1), Is.EqualTo(concert).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void SaveChangesToTheDatabaseWhenThereAreNoValidationErrors()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var controller = new ConcertsController(mockRavenSession, Substitute.For<IConcertDetailsService>());
                var concert = new Concert(1, "Foo", new DateTime(2012, 07, 31, 14, 24, 00), "Bar");

                controller.Create(new ConcertViewModel
                {
                    Date = concert.DateAndStartTime,
                    StartTime = concert.DateAndStartTime,
                    Location = concert.Location,
                    Title = concert.Title
                });

                mockRavenSession.Received().SaveChanges();
            }
        }

        [TestFixture]
        public class ListShould
        {
            [Test]
            public void ShowTheListView()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.List();

                result.AssertViewRendered().ForView("List");
            }
        }

        [TestFixture]
        public class EditShouldOnGet
        {
            [Test]
            public void ShowTheEditView()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), concertDetailsService);

                var result = controller.Edit(1);

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void ShowTheEditViewWithAConcertViewModel()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), concertDetailsService);

                var result = controller.Edit(1);

                result.AssertViewRendered().WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ShowTheEditViewForTheConcertSpecified()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), concertDetailsService);
                var viewModel = new ConcertViewModel
                {
                    Id = 1,
                    Title = "Test Concert",
                    Date = DateTime.MinValue,
                    StartTime = DateTime.MinValue,
                    Location = "Venue",
                    IsPublished = false
                };

                var result = controller.Edit(1) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(viewModel).Using(new ConcertViewModelEqualityComparer()));
            }

            [Test]
            public void CallGetConcertOnTheConcertDetailsServiceWithTheSpecifiedId()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), concertDetailsService);

                const int concertId = 1;
                controller.Edit(concertId);

                concertDetailsService.Received().GetConcert(concertId);
            }

            [Test]
            public void ThrowA404NotFoundIfTheRequestedConcertIdIsUnknown()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Edit(23);

                result.AssertResultIs<HttpNotFoundResult>();
            }

            private IConcertDetailsService GetMockConcertDetailsService()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetConcert(1).ReturnsForAnyArgs(new Concert(1, "Test Concert", DateTime.MinValue, "Venue"));
                return concertDetailsService;
            }
        }

        [TestFixture]
        public class EditShouldOnPost : RavenTest
        {
            private Concert existingConcert;

            [Test]
            public void RedirectToTheListViewIfNoErrorsOccurred()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void RedisplayTheEditViewWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void RedisplayTheEditViewWithTheProvidedViewModelWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>()
                    );
                controller.ModelState.AddModelError("Date", "Not a date");

                var concertToSave1 = new ConcertViewModel();
                var result = controller.Edit(1, concertToSave1) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(concertToSave1));
            }

            [Test]
            public void SaveTheChangesToTheDatabaseIfNoErrorsOccurred()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                var viewModel = existingConcert.ToViewModel<Concert, ConcertViewModel>();
                const string editedTitle = "New Title";
                viewModel.Title = editedTitle;

                controller.Edit(1, viewModel);

                var editedConcert = new Concert(existingConcert.Id, editedTitle, existingConcert.DateAndStartTime, existingConcert.Location);
                Assert.That(Session.Load<Concert>(1), Is.EqualTo(editedConcert).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void CallSaveChangesOnTheRavenSession()
            {
                var documentSession = Substitute.For<IDocumentSession>();
                var controller = new ConcertsController(documentSession, Substitute.For<IConcertDetailsService>());

                controller.Edit(1, new ConcertViewModel());

                documentSession.Received().SaveChanges();
            }

            [SetUp]
            public void CreateSampleData()
            {
                existingConcert = new Concert(1, "Foo", new DateTime(2012, 08, 04, 20, 00, 00), "Bar");

                using (var sampleDataSession = Store.OpenSession())
                {
                    sampleDataSession.Store(existingConcert);
                    sampleDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class ArchiveShould : RavenTest
        {
            private IReadOnlyCollection<Concert> concerts2009;

            [Test]
            public void ReturnArchiveView()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Archive(2009);

                result.AssertViewRendered().ForView("Archive");
            }

            [Test]
            public void ReturnArchiveViewWithConcerts()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Archive(2009);

                result.AssertViewRendered().WithViewData<IReadOnlyCollection<Concert>>();
            }

            [Test]
            public void ReturnArchiveWithTheConcertsFromTheRequestedSeason()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                IReadOnlyCollection<Concert> expectedData = concerts2009.ToArray();

                var result = controller.Archive(2009) as ViewResult;
                var viewModel = result.Model as IReadOnlyCollection<Concert>;

                Assert.That(viewModel, Is.EqualTo(expectedData).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void ReturnOnlyPublishedConcertsFromTheRequestedSeason()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());
                var unpublishedConcert = new Concert(1, "Unpublished 2009 Concert", new DateTime(2009, 11, 14), "West Road Concert Hall");
                Session.Store(unpublishedConcert);
                Session.SaveChanges();

                var result = controller.Archive(2009) as ViewResult;
                var viewModel = result.Model as IReadOnlyCollection<Concert>;

                Assert.That(viewModel, Is.Not.Contains(unpublishedConcert).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void SetTheConcertSeasonPropertyInTheViewBag()
            {
                var controller = new ConcertsController(Session, Substitute.For<IConcertDetailsService>());

                var result = controller.Archive(2009) as ViewResult;

                Assert.That(result.ViewBag.ConcertSeason, Is.EqualTo("2009-2010"));
            }

            [SetUp]
            public void CreateTestData()
            {
                concerts2009 = new List<Concert>
                {
                    new Concert(2, "Michaelmas", new DateTime(2009, 11, 13), "West Road Concert Hall"), 
                    new Concert(3, "Lent", new DateTime(2010, 02, 26), "West Road Concert Hall"), 
                    new Concert(4, "Spring", new DateTime(2010, 04, 2), "West Road Concert Hall"), 
                    new Concert(5, "Summer", new DateTime(2010, 06, 25), "West Road Concert Hall")
                };

                var concerts2009List = concerts2009.ToList();
                concerts2009List.ForEach(c => c.Publish());

                using (var sampleDataSession = Store.OpenSession())
                {
                    concerts2009List.ForEach(sampleDataSession.Store);          
                    sampleDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class ArchivedShould
        {
            [Test]
            public void ReturnTheArchiveIndexView()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>(), Substitute.For<IConcertDetailsService>());

                var result = controller.Archived();

                result.AssertViewRendered().ForView("ArchiveIndex");
            } 
        }
    }
}

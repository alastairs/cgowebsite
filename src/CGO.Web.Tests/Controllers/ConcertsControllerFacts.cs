using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Web.Controllers;
using CGO.Web.Mappers;
using CGO.Web.Models;
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
                var controller = new ConcertsController(Session);

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsInAscendingOrderByDate()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Index() as ViewResult;
                var concertsDisplayed = result.Model as IEnumerable<Concert>;

                Assert.That(concertsDisplayed, Is.EqualTo(concerts.OrderBy(c => c.DateAndStartTime)).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void DisplayTheConcertsFromTheDatabase()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Index() as ViewResult;

                Assert.That(result.Model, Is.EquivalentTo(concerts).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void DisplayOnlyConcertsInTheFuture()
            {
                var pastConcert = new Concert(5, "Concert in the past", new DateTime(2011, 11, 18, 20, 00, 00), "West Road Concert Hall");
                Session.Store(pastConcert);
                Session.SaveChanges();
                
                var controller = new ConcertsController(Session);
                
                var result = controller.Index() as ViewResult;

                Assert.That(result.Model, Is.Not.Contains(pastConcert).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void DisplayOnlyPublishedConcerts()
            {
                var unpublishedConcert = new Concert(5, "Unpublished concert", new DateTime(2101, 01, 18, 20, 00, 00), "West Road Concert Hall");
                Session.Store(unpublishedConcert);
                Session.SaveChanges();

                var controller = new ConcertsController(Session);

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
                var controller = new ConcertsController(Session);
                builder.InitializeController(controller);

                var result = controller.Index();

                result.AssertActionRedirect().ToAction("Index").ToController("Home");
            }

            [Test]
            public void DisplayTheCreateViewIfTheRequestIsAuthenticated()
            {
                var builder = new TestControllerBuilder();
                var controller = new ConcertsController(Session);
                builder.InitializeController(controller);
                controller.Request.Stub(r => r.IsAuthenticated).Return(true); // Have to use RhinoMocks here, as that's what MvcContrib uses
                
                var result = controller.Index();

                result.AssertViewRendered().ForView("List");
            }
        }

        [TestFixture]
        public class DetailsShould : RavenTest
        {
            [Test]
            public void RenderTheDetailsView()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Details(1) as ViewResult;
                var concert = result.Model as Concert;
                
                Assert.That(concert.Id, Is.EqualTo(1));
            }

            [Test]
            public void ReturnA404NotFoundWhenTheConcertDoesntExist()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Details(2);

                result.AssertResultIs<HttpNotFoundResult>();
            }

            [SetUp]
            public void CreateSampleData()
            {
                using(var sampleDataSession = Store.OpenSession())
                {
                    sampleDataSession.Store(new Concert(1, "foo", DateTime.Now, "bar"));
                    sampleDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class CreateShould : RavenTest
        {
            [Test]
            public void ShowTheCreateViewWhenCalledViaAGetRequest()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Create(); // The parameterless overload is called on GET.

                result.AssertViewRendered().ForView("Create");
            }

            [Test]
            public void ShowTheCreateViewWithTheSuppliedModelWhenThereAreValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());
                controller.ViewData.ModelState.AddModelError("Title", "Please enter a title");

                var result = controller.Create(new ConcertViewModel()); // The overload with ConcertViewModel parameter is called on POST.

                result.AssertViewRendered().ForView("Create").WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ReturnToTheListOfConcertsWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Create(new ConcertViewModel());

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void StoreTheConcertInTheDatabaseWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Session);
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
                var controller = new ConcertsController(mockRavenSession);
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
        public class ListShould : RavenTest
        {
            private Concert sampleConcert;

            [Test]
            public void ShowTheListView()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.List();

                result.AssertViewRendered().ForView("List");
            }

            [SetUp]
            public void CreateSampleData()
            {
                sampleConcert = new Concert(1, "Foo", new DateTime(2012, 07, 31, 13, 40, 00), "Bar");

                using (var sampleDataSession = Store.OpenSession())
                {
                    sampleDataSession.Store(sampleConcert);
                    sampleDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class EditShouldOnGet : RavenTest
        {
            private Concert concertToEdit;

            [Test]
            public void ShowTheEditView()
            {
                var documentSession = GetMockDocumentSession();
                var controller = new ConcertsController(documentSession);

                var result = controller.Edit(1);

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void ShowTheEditViewWithAConcertViewModel()
            {
                var documentSession = GetMockDocumentSession();
                var controller = new ConcertsController(documentSession);

                var result = controller.Edit(1);

                result.AssertViewRendered().WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ShowTheEditViewForTheConcertSpecified()
            {
                var documentSession = GetMockDocumentSession();
                var controller = new ConcertsController(documentSession);
                var viewModel = new ConcertViewModel
                {
                    Id = 1,
                    Title = "Foo",
                    Date = new DateTime(2012, 08, 04, 20, 00, 00),
                    StartTime = new DateTime(2012, 08, 04, 20, 00, 00),
                    Location = "Bar",
                    IsPublished = false
                };

                var result = controller.Edit(1) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(viewModel).Using(new ConcertViewModelEqualityComparer()));
            }

            [Test]
            public void CallLoadOnTheRavenSessionWithTheSpecifiedId()
            {
                var documentSession = GetMockDocumentSession();
                var controller = new ConcertsController(documentSession);

                const int concertDocumentToLoad = 1;
                controller.Edit(concertDocumentToLoad);

                documentSession.Received().Load<Concert>(concertDocumentToLoad);
            }

            [Test]
            public void RetrieveTheConcertFromTheDatabase()
            {
                var controller = new ConcertsController(Session);
                var viewModel = new ConcertViewModel
                {
                    Id = concertToEdit.Id,
                    Title = concertToEdit.Title,
                    Date = concertToEdit.DateAndStartTime,
                    StartTime = concertToEdit.DateAndStartTime,
                    Location = concertToEdit.Location
                };

                var result = controller.Edit(2) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(viewModel).Using(new ConcertViewModelEqualityComparer()));
            }

            [Test]
            public void ThrowA404NotFoundIfTheRequestedConcertIdIsUnknown()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Edit(23);

                result.AssertResultIs<HttpNotFoundResult>();
            }

            [SetUp]
            public void CreateSampleData()
            {
                concertToEdit = new Concert(2, "Bar", new DateTime(2012, 08, 04, 20, 00, 00), "Foo");

                using (var session = Store.OpenSession())
                {
                    session.Store(concertToEdit);
                    session.SaveChanges();
                }
            }

            private static IDocumentSession GetMockDocumentSession()
            {
                var documentSession = Substitute.For<IDocumentSession>();
                documentSession.Load<Concert>(1).ReturnsForAnyArgs(new Concert(1, "Foo", new DateTime(2012, 08, 04, 20, 00, 00), "Bar"));

                return documentSession;
            }
        }

        [TestFixture]
        public class EditShouldOnPost : RavenTest
        {
            private Concert existingConcert;

            [Test]
            public void RedirectToTheListViewIfNoErrorsOccurred()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void RedisplayTheEditViewWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void RedisplayTheEditViewWithTheProvidedViewModelWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var concertToSave1 = new ConcertViewModel();
                var result = controller.Edit(1, concertToSave1) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(concertToSave1));
            }

            [Test]
            public void SaveTheChangesToTheDatabaseIfNoErrorsOccurred()
            {
                var controller = new ConcertsController(Session);
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
                var controller = new ConcertsController(documentSession);

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
    }
}

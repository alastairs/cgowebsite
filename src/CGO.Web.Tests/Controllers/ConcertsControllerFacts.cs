using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Web.Controllers;
using CGO.Web.Models;
using CGO.Web.Tests.EqualityComparers;
using CGO.Web.ViewModels;

using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

using Raven.Client;
using Raven.Client.Linq;

namespace CGO.Web.Tests.Controllers
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class IndexShould
        {
            [Test]
            public void DisplayTheIndexView()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsInDescendingOrderByDate()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Index() as ViewResult;
                var concerts = result.Model as IEnumerable<Concert>;

                Assert.That(concerts.OrderByDescending(c => c.DateAndStartTime), Is.EqualTo(concerts));
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

                result.AssertViewRendered().ForView("List").WithViewData<IEnumerable<ConcertViewModel>>();
            }

            [Test]
            public void RetrieveAllConcertsFromTheDatabase()
            {
                var mockRavenSession = Substitute.For<IDocumentSession>();
                var mockQueryResult = Substitute.For<IRavenQueryable<Concert>>();
                mockRavenSession.Query<Concert>().ReturnsForAnyArgs(mockQueryResult);
                var controller = new ConcertsController(mockRavenSession);

                controller.List();

                mockRavenSession.Received().Query<Concert>();
            }

            [Test]
            public void ConvertTheRetrievedConcertsToConcertViewModels()
            {
                var controller = new ConcertsController(Session);
                var expectedViewModel = new[]
                {
                    new ConcertViewModel
                    {
                        Id = sampleConcert.Id,
                        Title = sampleConcert.Title,
                        Date = sampleConcert.DateAndStartTime,
                        StartTime = sampleConcert.DateAndStartTime,
                        Location = sampleConcert.Location
                    }
                };

                var result = controller.List() as ViewResult;

                Assert.That(result.Model, Is.EqualTo(expectedViewModel).Using(new ConcertViewModelEqualityComparer()));
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
        public class EditShouldOnPost
        {
            private ConcertViewModel concertToSave = new ConcertViewModel();

            [Test]
            public void RedirectToTheListViewIfNoErrorsOccurred()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Edit(1, concertToSave);

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void RedisplayTheEditViewWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var result = controller.Edit(1, concertToSave);

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void RedisplayTheEditViewWithTheProvidedViewModelWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var result = controller.Edit(1, concertToSave) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(concertToSave));
            }
        }
    }
}

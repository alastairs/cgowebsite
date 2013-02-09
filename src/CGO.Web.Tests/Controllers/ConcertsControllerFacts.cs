using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Domain;
using CGO.Web.Controllers;
using CGO.Web.Tests.EqualityComparers;
using CGO.Web.ViewModels;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;
using Rhino.Mocks;

namespace CGO.Web.Tests.Controllers
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class ConstructorShould
        {
            [Test]
            public void ThrowAnArgumentNullExceptionIfTheConcertDetailsServiceIsNull()
            {
                Assert.That(() => new ConcertsController(null, Substitute.For<IConcertsSeasonService>()),
                            Throws.InstanceOf<ArgumentNullException>());
            } 
            
            [Test]
            public void ThrowAnArgumentNullExceptionIfTheConcertsSeasonServiceIsNull()
            {
                Assert.That(() => new ConcertsController(Substitute.For<IConcertDetailsService>(), null),
                            Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class WhenThereAreConcerts_IndexShould
        {
            [Test]
            public void DisplayTheIndexView()
            {
                var concertDetailsService = GetMockConcertDetailsService(new[] { new Concert(1, "Test Concert", DateTime.MinValue, "Venue") });
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsFromTheDatabase()
            {
                var expectedConcerts = new[] { new Concert(1, "Test Concert", DateTime.MinValue, "Venue") };
                var concertDetailsService = GetMockConcertDetailsService(expectedConcerts);
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Index() as ViewResult;

                Assert.That(result.Model, Is.EquivalentTo(expectedConcerts).Using(new ConcertEqualityComparer()));
            }

            private static IConcertDetailsService GetMockConcertDetailsService(IReadOnlyCollection<Concert> expectedConcerts)
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetFutureConcerts().Returns(expectedConcerts);
                return concertDetailsService;
            }
        }

        [TestFixture]
        public class WhenThereAreNoConcerts_IndexShould
        {
            [Test]
            public void DisplayTheSiteHomePageIfTheRequestIsAnonymous()
            {
                var builder = new TestControllerBuilder();
                var controller = new ConcertsController(GetMockConcertDetailsService(),
                                                        Substitute.For<IConcertsSeasonService>());
                builder.InitializeController(controller);

                var result = controller.Index();

                result.AssertActionRedirect().ToAction("Index").ToController("Home");
            }

            [Test]
            public void DisplayTheCreateViewIfTheRequestIsAuthenticated()
            {
                var builder = new TestControllerBuilder();
                var controller = new ConcertsController(GetMockConcertDetailsService(),
                                                        Substitute.For<IConcertsSeasonService>());
                builder.InitializeController(controller);
                controller.Request.Stub(r => r.IsAuthenticated).Return(true); // Have to use RhinoMocks here, as that's what MvcContrib uses
                
                var result = controller.Index();

                result.AssertViewRendered().ForView("List");
            }

            private static IConcertDetailsService GetMockConcertDetailsService()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetFutureConcerts().Returns(Enumerable.Empty<Concert>().ToList());
                return concertDetailsService;
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
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                const int requestedConcertId = 1;
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetConcert(requestedConcertId).Returns(sampleConcert);
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Details(requestedConcertId) as ViewResult;
                var concert = result.Model as Concert;
                
                Assert.That(concert.Id, Is.EqualTo(requestedConcertId));
            }

            [Test]
            public void ReturnA404NotFoundWhenTheConcertDoesntExist()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Details(2);

                result.AssertResultIs<HttpNotFoundResult>();
            }
        }

        [TestFixture]
        public class CreateShould
        {
            [Test]
            public void ShowTheCreateViewWhenCalledViaAGetRequest()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Create(); // The parameterless overload is called on GET.

                result.AssertViewRendered().ForView("Create");
            }

            [Test]
            public void ShowTheCreateViewWithTheSuppliedModelWhenThereAreValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());
                controller.ViewData.ModelState.AddModelError("Title", "Please enter a title");

                var result = controller.Create(new ConcertViewModel()); // The overload with ConcertViewModel parameter is called on POST.

                result.AssertViewRendered().ForView("Create").WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ReturnToTheListOfConcertsWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Create(new ConcertViewModel());

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void CallSaveOnTheConcertDetailsServiceWhenThereAreNoValidationErrors()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                controller.Create(new ConcertViewModel
                {
                    Date = new DateTime(2012, 07, 29, 20, 00, 00),
                    StartTime = new DateTime(2012, 07, 29, 20, 00, 00),
                    Location = "Bar",
                    Title = "Foo"
                });

                concertDetailsService.ReceivedWithAnyArgs(1).SaveConcert(null);
            }
        }

        [TestFixture]
        public class ListShould
        {
            [Test]
            public void ShowTheListView()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

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
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Edit(1);

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void ShowTheEditViewWithAConcertViewModel()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Edit(1);

                result.AssertViewRendered().WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ShowTheEditViewForTheConcertSpecified()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());
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
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                const int concertId = 1;
                controller.Edit(concertId);

                concertDetailsService.Received().GetConcert(concertId);
            }

            [Test]
            public void ThrowA404NotFoundIfTheRequestedConcertIdIsUnknown()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

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
        public class EditShouldOnPost
        {
            [Test]
            public void RedirectToTheListViewIfNoErrorsOccurred()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void RedisplayTheEditViewWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void RedisplayTheEditViewWithTheProvidedViewModelWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var concertToSave1 = new ConcertViewModel();
                var result = controller.Edit(1, concertToSave1) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(concertToSave1));
            }

            [Test]
            public void CallSaveOnTheConcertDetailsServiceIfNoErrorsOccurred()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());
                
                controller.Edit(1, new ConcertViewModel());

                concertDetailsService.ReceivedWithAnyArgs(1).SaveConcert(null);
            }
        }

        [TestFixture]
        public class ArchiveShould
        {
            [Test]
            public void ReturnArchiveView()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Archive(2009);

                result.AssertViewRendered().ForView("Archive");
            }

            [Test]
            public void ReturnArchiveViewWithConcerts()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Archive(2009);

                result.AssertViewRendered().WithViewData<IReadOnlyCollection<Concert>>();
            }

            [Test]
            public void ReturnArchiveWithTheConcertsFromTheRequestedSeasonFromTheService()
            {
                var concertsSeasonService = Substitute.For<IConcertsSeasonService>();
                var concert = new Concert(1, "Test Concert", DateTime.MinValue, "Venue");
                IReadOnlyCollection<Concert> expectedConcerts = new[] { concert };
                concertsSeasonService.GetConcertsInSeason(2009).Returns(expectedConcerts);
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        concertsSeasonService);

                var result = controller.Archive(2009) as ViewResult;
                var viewModel = result.Model as IReadOnlyCollection<Concert>;

                Assert.That(viewModel, Is.EqualTo(expectedConcerts).Using(new ConcertEqualityComparer()));
            }

            [Test]
            public void SetTheConcertSeasonPropertyInTheViewBag()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Archive(2009) as ViewResult;

                Assert.That(result.ViewBag.ConcertSeason, Is.EqualTo("2009-2010"));
            }
        }

        [TestFixture]
        public class ArchivedShould
        {
            [Test]
            public void ReturnTheArchiveIndexView()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>(),
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Archived();

                result.AssertViewRendered().ForView("ArchiveIndex");
            } 
        }
    }
}

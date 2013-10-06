using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Domain;
using CGO.Web.Controllers;
using CGO.Web.Mappers;
using CGO.Web.Models;
using CGO.Web.Tests.EqualityComparers;
using CGO.Web.ViewModels;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

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

                result.AssertViewRendered().ForView("Index").WithViewData<ConcertsIndexViewModel>();
            }

            [Test]
            public void DisplayTheConcertsFromTheDatabase()
            {
                var expectedConcerts = new[] { new Concert(1, "Test Concert", DateTime.MinValue, "Venue") };
                var concertDetailsService = GetMockConcertDetailsService(expectedConcerts);
                var controller = new ConcertsController(concertDetailsService,
                                                        Substitute.For<IConcertsSeasonService>());

                var result = controller.Index() as ViewResult;

                var expected = new ConcertsIndexViewModel
                {
                    NextConcert = expectedConcerts.First().ToViewModel<Concert, ConcertViewModel>(),
                    UpcomingConcerts = Enumerable.Empty<ConcertViewModel>()
                };
                Assert.That(result.Model, Is.EqualTo(expected).Using(new ConcertsIndexViewModelEqualityComparer()));
            }

            private static IConcertDetailsService GetMockConcertDetailsService(IReadOnlyCollection<Concert> expectedConcerts)
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetFutureConcerts().Returns(expectedConcerts);
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

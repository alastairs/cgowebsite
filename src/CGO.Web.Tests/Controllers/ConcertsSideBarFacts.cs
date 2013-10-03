using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CGO.Domain;
using CGO.Domain.Entities;
using CGO.Domain.Services;
using CGO.Domain.Services.Application;
using CGO.Web.Controllers;
using CGO.Web.Infrastructure;
using NSubstitute;
using NUnit.Framework;

namespace CGO.Web.Tests.Controllers
{
    class ConcertsSideBarFacts
    {
        [TestFixture]
        public class ConstructorShould
        {
            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheUrlHelperIsNull()
            {
                Assert.That(() => new ConcertsSideBar(null, Substitute.For<IConcertsSeasonService>(), Substitute.For<IDateTimeProvider>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheConcertsSeasonServiceIsNull()
            {
                Assert.That(() => new ConcertsSideBar(Substitute.For<IUrlHelper>(), null, Substitute.For<IDateTimeProvider>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheDateTimeProviderIsNull()
            {
                Assert.That(() => new ConcertsSideBar(Substitute.For<IUrlHelper>(), Substitute.For<IConcertsSeasonService>(), null), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class GetSideBarSectionsShould : RavenTest
        {
            private IDateTimeProvider dateTimeProvider;

            [Test]
            public void ReturnTheFirstSectionForTheCurrentSeason()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  GetMockConcertsSeasonService(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.First().Title, Is.EqualTo("Current Season"));
            }

            [Test]
            public void CallGetConcertsInCurrentSeasonOnTheConcertsSeasonService()
            {
                var concertsSeasonService = GetMockConcertsSeasonService();
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  concertsSeasonService, dateTimeProvider);

                sideBar.GetSideBarSections().ToList();

                concertsSeasonService.Received(1).GetConcertsInCurrentSeason();
            }

            [Test]
            public void PresentTheCurrentSeasonConcertsInTheFirstSection()
            {
                var concertsSeasonService = GetMockConcertsSeasonService();
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  concertsSeasonService, dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.First().Links.Select(l => l.Title),
                            Is.EqualTo(concertsSeasonService.GetConcertsInCurrentSeason().Select(c => c.Title)));
            }

            [Test]
            public void ReturnTheSecondSectionForTheLastSeason()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  GetMockConcertsSeasonService(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Skip(1).First().Title, Is.EqualTo("Last Season"));
            }

            [Test]
            public void CallGetConcertsInPreviousSeasonOnTheConcertsSeasonService()
            {
                var concertsSeasonService = GetMockConcertsSeasonService();
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  concertsSeasonService, dateTimeProvider);

                sideBar.GetSideBarSections().ToList();

                concertsSeasonService.Received(1).GetConcertsInPreviousSeason();
            }

            [Test]
            public void PresentThePreviousSeasonConcertsAsTheSecondSection()
            {
                var concertsSeasonService = GetMockConcertsSeasonService();
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  concertsSeasonService, dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Skip(1).First().Links.Select(l => l.Title), 
                    Is.EqualTo(concertsSeasonService.GetConcertsInPreviousSeason().Select(c => c.Title)));
            }

            [Test]
            public void ReturnTheThirdSectionForTheArchive()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  GetMockConcertsSeasonService(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Skip(2).First().Title, Is.EqualTo("Older"));
            }

            [TestCase(0, "2009-10")]
            [TestCase(1, "2008-09")]
            [TestCase(2, "2007-08")]
            [TestCase(3, "2006-07")]
            [TestCase(4, "2005-06")]
            [TestCase(5, "2004-05")]
            public void ReturnALinkForEachArchivedSeason(int linkIndex, string linkYears)
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  GetMockConcertsSeasonService(), dateTimeProvider);

                var archiveLinks = sideBar.GetSideBarSections().Skip(2).First().Links;

                Assert.That(archiveLinks.Skip(linkIndex).First().Title, Is.EqualTo(linkYears + " Season"));
            }

            [Test]
            public void NotPresentTheCurrentSeasonIfThereAreNoConcerts()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Select(s => s.Title), Is.Not.Contains("Current Season"));
            }

            [Test]
            public void NotPresentTheLastSeasonIfThereAreNoConcerts()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Select(s => s.Title), Is.Not.Contains("Last Season"));
            }

            [SetUp]
            public void FixDateTimeNow()
            {
                dateTimeProvider = Substitute.For<IDateTimeProvider>();
                dateTimeProvider.Now.Returns(new DateTime(2012, 05, 01));
            }

            public void CreateSampleData(IEnumerable<Concert> concerts)
            {
                using (var sampleDataSession = Store.OpenSession())
                {
                    foreach (var concert in concerts)
                    {
                        sampleDataSession.Store(concert);
                    }

                    sampleDataSession.SaveChanges();
                }
            }

            private static IConcertsSeasonService GetMockConcertsSeasonService()
            {
                var concertsSeasonService = Substitute.For<IConcertsSeasonService>();
                concertsSeasonService.GetConcertsInCurrentSeason()
                                     .Returns(new[] { new Concert(1, "Current Season Concert", DateTime.MinValue, "Venue") } );
                concertsSeasonService.GetConcertsInPreviousSeason()
                                     .Returns(new[] { new Concert(2, "Previous Season Concert", DateTime.MinValue, "Venue") });
                
                return concertsSeasonService;
            }
        }

        private static IUrlHelper GetMockUrlHelper()
        {
            var mockUrlHelper = Substitute.For<IUrlHelper>();
            mockUrlHelper.Action(string.Empty, string.Empty, null)
                         .ReturnsForAnyArgs(callInfo => GetUrl(callInfo.Args()[0] as string, callInfo.Args()[1] as string, callInfo.Args()[2]));

            return mockUrlHelper;
        }

        private static string GetUrl(string actionName, string controllerName, object urlArgument)
        {
            var idArgument = GetItemFromObject(urlArgument, "id");
            if (idArgument != null)
            {
                return string.Format("/{1}/{0}/{2}", actionName,
                                 controllerName, idArgument);
            }

            return string.Format("/{1}/{0}?year={2}", actionName, controllerName, GetItemFromObject(urlArgument, "year"));
        }

        private static object GetItemFromObject(object urlArgument, string argName)
        {
            var properties = TypeDescriptor.GetProperties(urlArgument);

            return properties.Cast<PropertyDescriptor>()
                             .Where(property => property.Name == argName)
                             .Select(property => property.GetValue(urlArgument))
                             .FirstOrDefault();
        }
    }
}

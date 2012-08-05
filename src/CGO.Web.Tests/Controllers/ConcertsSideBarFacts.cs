using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CGO.Web.Controllers;
using CGO.Web.Infrastructure;
using CGO.Web.Models;
using CGO.Web.Tests.EqualityComparers;

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
                Assert.That(() => new ConcertsSideBar(null, Substitute.For<IDocumentSessionFactory>(), Substitute.For<IDateTimeProvider>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheDocumentSessionFactoryIsNull()
            {
                Assert.That(() => new ConcertsSideBar(Substitute.For<IUrlHelper>(), null, Substitute.For<IDateTimeProvider>()), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class GetSideBarSectionsShould : RavenTest
        {
            private IDateTimeProvider dateTimeProvider;

            [Test]
            public void ReturnTheFirstSectionForTheCurrentSeason()
            {
                var concerts = new[] { new Concert(1, "Concert", new DateTime(2012, 12, 01, 20, 00, 00), "Venue") };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.First().Title, Is.EqualTo("Current Season"));
            }

            [Test]
            public void PresentTheCurrentSeasonWithConcertsInTheFuture()
            {
                var futureConcert = new Concert(2, "Concert in the future", new DateTime(2012, 06, 29, 20, 00, 00), "Venue");
                var concerts = new[]
                {
                    new Concert(1, "Concert in the past", new DateTime(2011, 06, 27, 20, 00, 00), "Venue"),
                    futureConcert
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Current Season", new[]
                {
                    new SideBarLink(futureConcert.Title, "/Concerts/Details/" + futureConcert.Id, false)
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheCurrentSeasonWithNoConcertsFromTheNextSeason()
            {
                var futureConcert = new Concert(2, "Current Season concert in the future", new DateTime(2012, 06, 29, 20, 00, 00), "Venue");
                var concerts = new[]
                {
                    new Concert(1, "Next Season concert", new DateTime(2012, 12, 01, 20, 00, 00), "Venue"),
                    futureConcert
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Current Season", new[]
                {
                    new SideBarLink(futureConcert.Title, "/Concerts/Details/" + futureConcert.Id, false)
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheCurrentSeasonWithConcertsInThePast()
            {
                var pastConcert = new Concert(1, "Current Season concert in the past", new DateTime(2012, 04, 11, 20, 00, 00), "Venue");
                var concerts = new[]
                {
                    pastConcert,
                    new Concert(2, "Last Season concert", new DateTime(2011, 06, 27, 20, 00, 00), "Venue")
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Current Season", new[]
                {
                    new SideBarLink(pastConcert.Title, "/Concerts/Details/" + pastConcert.Id, false) 
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheCurrentSeasonSortedByDateAndStartTimeInAscendingOrder()
            {
                var concerts = new[]
                {
                    new Concert(2, "Current Season concert 2", new DateTime(2012, 06, 29, 20, 00, 00), "Venue"),
                    new Concert(1, "Current Season concert 1", new DateTime(2012, 04, 11, 20, 00, 00), "Venue")
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Current Season", new[]
                {
                    new SideBarLink("Current Season concert 1", "/Concerts/Details/1", false), 
                    new SideBarLink("Current Season concert 2", "/Concerts/Details/2", false) 
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheCurrentSeasonAsTheFirstSection()
            {
                var concerts = new[]
                {
                    new Concert(4, "Summer Concert", new DateTime(2012, 06, 29, 20, 00, 00), "Venue"),
                    new Concert(1, "Michaelmas Concert", new DateTime(2011, 11, 18, 20, 00, 00), "Venue"), 
                    new Concert(3, "Spring Concert", new DateTime(2012, 04, 11, 20, 00, 00), "Venue"), 
                    new Concert(2, "Lent Concert", new DateTime(2012, 03, 09, 20, 00, 00), "Venue")
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Current Season", new[]
                {
                    new SideBarLink("Michaelmas Concert", "/Concerts/Details/1", false), 
                    new SideBarLink("Lent Concert", "/Concerts/Details/2", false), 
                    new SideBarLink("Spring Concert", "/Concerts/Details/3", false), 
                    new SideBarLink("Summer Concert", "/Concerts/Details/4", false) 
                });

                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
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
        }

        private static IUrlHelper GetMockUrlHelper()
        {
            var mockUrlHelper = Substitute.For<IUrlHelper>();
            mockUrlHelper.Action(string.Empty, string.Empty, null)
                         .ReturnsForAnyArgs(callInfo => string.Format("/{1}/{0}/{2}", callInfo.Args()[0],
                                                                      callInfo.Args()[1], GetIdFromObject(callInfo.Args()[2])));

            return mockUrlHelper;
        }

        private static object GetIdFromObject(object arg)
        {
            var properties = TypeDescriptor.GetProperties(arg);

            return properties.Cast<PropertyDescriptor>()
                             .Where(property => property.Name == "id")
                             .Select(property => property.GetValue(arg))
                             .FirstOrDefault();
        }
    }
}

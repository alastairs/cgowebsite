using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CGO.Domain;
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
                Assert.That(() => new ConcertsSideBar(null, Substitute.For<IDocumentSessionFactory>(),
                                                      Substitute.For<IConcertsSeasonService>(), Substitute.For<IDateTimeProvider>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheDocumentSessionFactoryIsNull()
            {
                Assert.That(() => new ConcertsSideBar(Substitute.For<IUrlHelper>(), null,
                                                      Substitute.For<IConcertsSeasonService>(), Substitute.For<IDateTimeProvider>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheDateTimeProviderIsNull()
            {
                Assert.That(() => new ConcertsSideBar(Substitute.For<IUrlHelper>(), Substitute.For<IDocumentSessionFactory>(),
                                                      Substitute.For<IConcertsSeasonService>(), null), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class GetSideBarSectionsShould : RavenTest
        {
            private IDateTimeProvider dateTimeProvider;

            [Test]
            public void ReturnTheFirstSectionForTheCurrentSeason()
            {
                var concerts = new[] { new Concert(1, "Concert", new DateTime(2012, 06, 29, 20, 00, 00), "Venue") };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

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
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

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
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

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
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

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
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

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
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

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

            [Test]
            public void ReturnTheSecondSectionForTheLastSeason()
            {
                var concerts = new[]
                                   {
                                       new Concert(1, "Current Season Concert", new DateTime(2012, 06, 29, 20, 00, 00), "Venue"), 
                                       new Concert(2, "Concert", new DateTime(2011, 06, 01, 20, 00, 00), "Venue")
                                   };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Skip(1).First().Title, Is.EqualTo("Last Season"));
            }

            [Test]
            public void PresentTheLastSeasonWithTheMichaelmasConcert()
            {
                var michaelmasConcert = new Concert(2, "Michaelmas Concert", new DateTime(2010, 11, 26, 20, 00, 00), "Venue");
                var concerts = new[]
                {
                    michaelmasConcert
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Last Season", new[]
                {
                    new SideBarLink(michaelmasConcert.Title, "/Concerts/Details/" + michaelmasConcert.Id, false)
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheLastSeasonWithTheSummerConcert()
            {
                var summerConcert = new Concert(2, "Summer Concert", new DateTime(2011, 06, 24, 20, 00, 00), "Venue");
                var concerts = new[]
                {
                    summerConcert
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Last Season", new[]
                {
                    new SideBarLink(summerConcert.Title, "/Concerts/Details/" + summerConcert.Id, false)
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheLastSeasonWithConcertsInTheCurrentYear()
            {
                var summerConcert = new Concert(2, "Summer Concert", new DateTime(2011, 06, 24, 20, 00, 00), "Venue");
                var concerts = new[]
                {
                    summerConcert
                };
                CreateSampleData(concerts);
                dateTimeProvider.Now.Returns(new DateTime(2011, 12, 01));
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Last Season", new[]
                {
                    new SideBarLink(summerConcert.Title, "/Concerts/Details/" + summerConcert.Id, false)
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheLastSeasonSortedByDateAndStartTimeInAscendingOrder()
            {
                var concerts = new[]
                {
                    new Concert(2, "Last Season concert 2", new DateTime(2011, 06, 24, 20, 00, 00), "Venue"),
                    new Concert(1, "Last Season concert 1", new DateTime(2011, 04, 15, 20, 00, 00), "Venue")
                };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSideBar = new SideBarSection("Last Season", new[]
                {
                    new SideBarLink("Last Season concert 1", "/Concerts/Details/1", false), 
                    new SideBarLink("Last Season concert 2", "/Concerts/Details/2", false) 
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSideBar).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheLastSeasonAsTheSecondSection()
            {
                var currentSeasonConcerts = new[]
                {
                    new Concert(4, "Summer Concert", new DateTime(2012, 06, 29, 20, 00, 00), "Venue"),
                    new Concert(1, "Michaelmas Concert", new DateTime(2011, 11, 18, 20, 00, 00), "Venue"),
                    new Concert(3, "Spring Concert", new DateTime(2012, 04, 11, 20, 00, 00), "Venue"),
                    new Concert(2, "Lent Concert", new DateTime(2012, 03, 09, 20, 00, 00), "Venue")
                };
                var lastSeasonConcerts = new[]
                {
                    new Concert(8, "Summer Concert", new DateTime(2011, 06, 24, 20, 00, 00), "Venue"),
                    new Concert(5, "Michaelmas Concert", new DateTime(2010, 11, 26, 20, 00, 00), "Venue"),
                    new Concert(7, "Spring Concert", new DateTime(2011, 04, 15, 20, 00, 00), "Venue"),
                    new Concert(6, "Lent Concert", new DateTime(2011, 03, 11, 20, 00, 00), "Venue")
                };
                CreateSampleData(currentSeasonConcerts.Concat(lastSeasonConcerts));
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var lastSeasonSection = new SideBarSection("Last Season", new[]
                {
                    new SideBarLink("Michaelmas Concert", "/Concerts/Details/5", false),
                    new SideBarLink("Lent Concert", "/Concerts/Details/6", false),
                    new SideBarLink("Spring Concert", "/Concerts/Details/7", false),
                    new SideBarLink("Summer Concert", "/Concerts/Details/8", false)
                });
                Assert.That(sections.Skip(1).First(), Is.EqualTo(lastSeasonSection).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void ReturnTheThirdSectionForTheArchive()
            {
                var concerts = new[]
                                   {
                                       new Concert(3, "Current Season Concert", new DateTime(2012, 06, 29, 20, 00, 00), "Venue"), 
                                       new Concert(2, "Last Season Concert", new DateTime(2011, 06, 11, 20, 00, 00), "Venue"), 
                                       new Concert(1, "Concert", new DateTime(2010, 06, 25, 20, 00, 00), "Venue")
                                   };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Skip(2).First().Title, Is.EqualTo("Older"));
            }

            [Test]
            public void ReturnALinkForThe2009To2010Season()
            {
                var concerts = new[] { new Concert(1, "Concert", new DateTime(2010, 06, 25, 20, 00, 00), "Venue") };
                CreateSampleData(concerts);
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var expectedSection = new SideBarSection("Older", new[]
                {
                    new SideBarLink("2009-10 Season", "/Concerts/Archive?year=2009", false)
                });
                Assert.That(sections.First(), Is.EqualTo(expectedSection).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void PresentTheArchiveAsTheThirdSection()
            {
                var currentSeasonConcerts = new[]
                {
                    new Concert(4, "Summer Concert", new DateTime(2012, 06, 29, 20, 00, 00), "Venue"),
                    new Concert(1, "Michaelmas Concert", new DateTime(2011, 11, 18, 20, 00, 00), "Venue"),
                    new Concert(3, "Spring Concert", new DateTime(2012, 04, 11, 20, 00, 00), "Venue"),
                    new Concert(2, "Lent Concert", new DateTime(2012, 03, 09, 20, 00, 00), "Venue")
                };
                var lastSeasonConcerts = new[]
                {
                    new Concert(8, "Summer Concert", new DateTime(2011, 06, 24, 20, 00, 00), "Venue"),
                    new Concert(5, "Michaelmas Concert", new DateTime(2010, 11, 26, 20, 00, 00), "Venue"),
                    new Concert(7, "Spring Concert", new DateTime(2011, 04, 15, 20, 00, 00), "Venue"),
                    new Concert(6, "Lent Concert", new DateTime(2011, 03, 11, 20, 00, 00), "Venue")
                };
                var archivedConcerts = new[]
                {
                    new Concert(9, "Summer Concert 2010", new DateTime(2010, 06, 25, 20, 00, 00), "Venue"),
                    new Concert(10, "Summer Concert 2009", new DateTime(2009, 06, 29, 20, 00, 00), "Venue"),
                    new Concert(11, "Summer Concert 2008", new DateTime(2008, 06, 27, 20, 00, 00), "Venue"),
                    new Concert(12, "Summer Concert 2007", new DateTime(2007, 06, 14, 20, 00, 00), "Venue")
                };
                CreateSampleData(currentSeasonConcerts.Concat(lastSeasonConcerts).Concat(archivedConcerts));
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                var archiveSection = new SideBarSection("Older", new[]
                {
                    new SideBarLink("2009-10 Season", "/Concerts/Archive?year=2009", false), 
                    new SideBarLink("2008-09 Season", "/Concerts/Archive?year=2008", false), 
                    new SideBarLink("2007-08 Season", "/Concerts/Archive?year=2007", false),
                    new SideBarLink("2006-07 Season", "/Concerts/Archive?year=2006", false)
                });
                Assert.That(sections.Skip(2).First(), Is.EqualTo(archiveSection).Using(new SideBarSectionEqualityComparer()));
            }

            [Test]
            public void NotPresentTheCurrentSeasonIfThereAreNoConcerts()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Select(s => s.Title), Is.Not.Contains("Current Season"));
            }

            [Test]
            public void NotPresentTheLastSeasonIfThereAreNoConcerts()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Select(s => s.Title), Is.Not.Contains("Last Season"));
            }

            [Test]
            public void NotPresentTheArchiveSectionIfThereAreNoConcerts()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store),
                                                  Substitute.For<IConcertsSeasonService>(), dateTimeProvider);

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.Select(s => s.Title), Is.Not.Contains("Older"));
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

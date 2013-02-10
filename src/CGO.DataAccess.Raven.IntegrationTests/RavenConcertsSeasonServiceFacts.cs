using System;
using System.Collections.Generic;
using System.Linq;
using CGO.Domain;
using NSubstitute;
using NUnit.Framework;

namespace CGO.DataAccess.Raven.IntegrationTests
{
    class RavenConcertsSeasonServiceFacts
    {
        [TestFixture]
        public class GetConcertsInSeasonShould : RavenTest
        {
            [Test]
            public void ReturnTheConcertsAfter31JulyInTheSpecifiedYear()
            {
                var expectedConcerts = CreateConcerts();
                var concertsSeasonService = new RavenConcertsSeasonService(Session, Substitute.For<IDateTimeProvider>());
                
                var actualConcerts = concertsSeasonService.GetConcertsInSeason(2009);

                Assert.That(actualConcerts.Select(c => c.Id), Is.EqualTo(expectedConcerts.Skip(1).Select(c => c.Id)));
            }

            [Test]
            public void ReturnTheConcertsBefore1AugustInTheFollowingYear()
            {
                var expectedConcerts = CreateConcerts();
                var concertsSeasonService = new RavenConcertsSeasonService(Session, Substitute.For<IDateTimeProvider>());

                var actualConcerts = concertsSeasonService.GetConcertsInSeason(2008);

                Assert.That(actualConcerts.Select(c => c.Id), Is.EqualTo(expectedConcerts.Take(1).Select(c => c.Id)));
            }

            [Test]
            public void ReturnOnlyPublishedConcerts()
            {
                const int unpublishedConcertId = 3;
                CreateConcerts();
                Session.Store(new Concert(unpublishedConcertId, "Unpublished Concert", new DateTime(2010, 02, 26), "West Road Concert Hall"));
                Session.SaveChanges();
                var concertsSeasonService = new RavenConcertsSeasonService(Session, Substitute.For<IDateTimeProvider>());

                var seasonConcerts = concertsSeasonService.GetConcertsInSeason(2009);

                Assert.That(seasonConcerts.Select(c => c.Id), Is.Not.Contains(unpublishedConcertId));
            }

            private IEnumerable<Concert> CreateConcerts()
            {
                var concerts = new List<Concert>
                {
                    new Concert(1, "Pre-2009 Season Concert", new DateTime(2009, 06, 29), "West Road Concert Hall"), 
                    new Concert(2, "Michaelmas", new DateTime(2009, 11, 13), "West Road Concert Hall"),
                };

                concerts.ForEach(c => c.Publish());
                using (var session = Store.OpenSession())
                {
                    concerts.ForEach(session.Store);
                    session.SaveChanges();
                }

                return concerts;
            }
        }

        [TestFixture]
        public class Before1AugustGetConcertsInCurrentSeasonShould : RavenTest
        {
            [Test]
            public void ReturnConcertsInThePreviousYear()
            {
                var concert2009Season = new Concert(1, "2009-10 Season Concert", new DateTime(2009, 11, 26), "Venue");
                concert2009Season.Publish();
                CreateSampleData(concert2009Season);
                var dateTimeProvider = GetMockDateTimeProvider();
                var concertsSeasonService = new RavenConcertsSeasonService(Session, dateTimeProvider);

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                var expectedConcerts = new[] { concert2009Season.Id };
                Assert.That(currentSeasonConcerts.Select(c => c.Id), Is.EqualTo(expectedConcerts));
            }

            [Test]
            public void ReturnConcertsInTheCurrentYear()
            {
                var concert2009Season = new Concert(2, "2009-10 Season Concert", new DateTime(2010, 03, 09), "Venue");
                concert2009Season.Publish();
                CreateSampleData(concert2009Season);
                var dateTimeProvider = GetMockDateTimeProvider();
                var concertsSeasonService = new RavenConcertsSeasonService(Session, dateTimeProvider);

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                var expectedConcerts = new[] { concert2009Season.Id };
                Assert.That(currentSeasonConcerts.Select(c => c.Id), Is.EqualTo(expectedConcerts));
            }

            [Test]
            public void NotReturnConcertsInThePreviousSeason()
            {
                var concert2008Season = new Concert(2, "2008-09 Season Concert", new DateTime(2009, 03, 09), "Venue");
                concert2008Season.Publish();
                CreateSampleData(concert2008Season);
                var dateTimeProvider = GetMockDateTimeProvider();
                var concertsSeasonService = new RavenConcertsSeasonService(Session, dateTimeProvider);

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                Assert.That(currentSeasonConcerts.Select(c => c.Id), Is.Not.Contains(concert2008Season.Id));
            }
            private void CreateSampleData(params Concert[] concerts)
            {
                using (var sampleDataSession = Store.OpenSession())
                {
                    Array.ForEach(concerts, sampleDataSession.Store);
                    
                    sampleDataSession.SaveChanges();
                }
            }

            private static IDateTimeProvider GetMockDateTimeProvider()
            {
                var dateTimeProvider = Substitute.For<IDateTimeProvider>();
                dateTimeProvider.Now.Returns(new DateTime(2010, 07, 31));
                return dateTimeProvider;
            }
        }
    }
}

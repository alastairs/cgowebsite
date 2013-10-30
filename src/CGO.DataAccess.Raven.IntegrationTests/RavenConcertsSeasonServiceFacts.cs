﻿using System;
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
        public class Before1SeptemberGetConcertsInCurrentSeasonShould : RavenTest
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

            [Test]
            public void OnlyReturnPublishedConcerts()
            {
                var concert2009Season = new Concert(2, "2009-10 Season Concert", new DateTime(2010, 03, 09), "Venue");
                CreateSampleData(concert2009Season);
                var dateTimeProvider = GetMockDateTimeProvider();
                var concertsSeasonService = new RavenConcertsSeasonService(Session, dateTimeProvider);

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                Assert.That(currentSeasonConcerts.Select(c => c.Id), Is.Not.Contains(concert2009Season.Id));
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
                dateTimeProvider.Now.Returns(new DateTime(2010, 08, 31));
                return dateTimeProvider;
            }
        }

        [TestFixture]
        public class After31AugustGetConcertsInCurrentSeasonShould : RavenTest
        {
            [TestCase(09)]
            [TestCase(10)]
            public void ReturnFutureConcerts(int month)
            {
                var concert2010Season = new Concert(1, "2010-11 Season Concert", new DateTime(2010, 11, 26), "Venue");
                concert2010Season.Publish();
                CreateSampleData(concert2010Season);
                var concertsSeasonService = new RavenConcertsSeasonService(Session, GetMockDateTimeProvider(month));

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                Assert.That(currentSeasonConcerts.Single().Id, Is.EqualTo(concert2010Season.Id));
            }

            [Test]
            public void NotReturnPastSeasonConcerts()
            {
                var concert2009Season = new Concert(1, "2009-10 Season Concert", new DateTime(2010, 02, 26), "Venue");
                concert2009Season.Publish();
                CreateSampleData(concert2009Season);
                var concertsSeasonService = new RavenConcertsSeasonService(Session, GetMockDateTimeProvider());

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                Assert.That(currentSeasonConcerts.Select(c => c.Id), Is.Not.Contains(concert2009Season.Id));
            }

            [Test]
            public void NotReturnFutureSeasonConcerts()
            {
                var concert2011Season = new Concert(1, "2011-12 Season Concert", new DateTime(2011, 11, 18), "Venue");
                concert2011Season.Publish();
                CreateSampleData(concert2011Season);
                var concertsSeasonService = new RavenConcertsSeasonService(Session, GetMockDateTimeProvider());

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                Assert.That(currentSeasonConcerts.Select(c => c.Id), Is.Not.Contains(concert2011Season.Id));
            }

            [Test]
            public void OnlyReturnPublishedConcerts()
            {
                var unpublishedConcert = new Concert(1, "Unpublished 2010-11 Season Concert", new DateTime(2011, 03, 08), "Venue");
                CreateSampleData(unpublishedConcert);
                var concertsSeasonService = new RavenConcertsSeasonService(Session, GetMockDateTimeProvider());

                var currentSeasonConcerts = concertsSeasonService.GetConcertsInCurrentSeason();

                Assert.That(currentSeasonConcerts.Select(c => c.Id), Is.Not.Contains(unpublishedConcert.Id));
            }

            private void CreateSampleData(params Concert[] concerts)
            {
                using (var sampleDataSession = Store.OpenSession())
                {
                    Array.ForEach(concerts, sampleDataSession.Store);

                    sampleDataSession.SaveChanges();
                }
            }

            private static IDateTimeProvider GetMockDateTimeProvider(int month = 9)
            {
                var dateTimeProvider = Substitute.For<IDateTimeProvider>();
                dateTimeProvider.Now.Returns(new DateTime(2010, month, 01));
                return dateTimeProvider;
            }
        }

        [TestFixture]
        public class GetConcertsInPreviousSeasonShould : RavenTest
        {
            [Test]
            public void ReturnConcertsThatTookPlaceInThePreviousSeason()
            {
                var expectedConcert = new Concert(1, "Russian Heritage", new DateTime(2012, 12, 01), "Venue");
                expectedConcert.Publish();
                CreateSampleData(expectedConcert);
                var concertsSeasonService = new RavenConcertsSeasonService(Session, GetMockDateTimeProvider());

                var previousSeasonConcerts = concertsSeasonService.GetConcertsInPreviousSeason();

                Assert.That(previousSeasonConcerts.Select(c => c.Id).ToArray(), Is.EqualTo(new[] { expectedConcert.Id }));
            }

            [Test]
            public void NotReturnConcertsThatAreTakingPlaceInTheCurrentSeason()
            {
                var excludedConcert = new Concert(1, "La Bohème", new DateTime(2013, 11, 30), "Venue");
                excludedConcert.Publish();
                CreateSampleData(excludedConcert);
                var concertsSeasonService = new RavenConcertsSeasonService(Session, GetMockDateTimeProvider());

                var previousSeasonConcerts = concertsSeasonService.GetConcertsInPreviousSeason();

                Assert.That(previousSeasonConcerts.Select(c => c.Id), Is.Not.Contains(excludedConcert));
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
                dateTimeProvider.Now.Returns(new DateTime(2013, 10, 30));
                return dateTimeProvider;
            }
        }
    }
}

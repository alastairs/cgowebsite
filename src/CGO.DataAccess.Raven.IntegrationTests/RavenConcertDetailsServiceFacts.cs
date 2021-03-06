﻿using System;
using System.Linq;
using CGO.Domain;
using NSubstitute;
using NUnit.Framework;

namespace CGO.DataAccess.Raven.IntegrationTests
{
    public class RavenConcertDetailsServiceFacts
    {
        [TestFixture]
        public class GetConcertShould : RavenTest
        {
            [Test]
            public void ReturnTheRequestedConcert()
            {
                var concertDetailService = new RavenConcertDetailsService(Session, new DateTimeProvider());
                const int concertId = 1;

                var returnedConcert = concertDetailService.GetConcert(concertId);

                Assert.That(returnedConcert.Id, Is.EqualTo(concertId));
            }

            [SetUp]
            public void CreateTestData()
            {
                using (var testDataSession = Store.OpenSession())
                {
                    testDataSession.Store(new Concert(1, "Test Concert", DateTime.MinValue, "Venue"));
                    testDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class GetFutureConcertsShould : RavenTest
        {
            [Test]
            public void ReturnOnlyConcertsInTheFuture()
            {
                var concertDetailsService = new RavenConcertDetailsService(Session, GetMockDateTimeProvider());

                var futureConcerts = concertDetailsService.GetFutureConcerts();

                Assert.That(futureConcerts.Select(c => c.Id), Is.EquivalentTo(new[] {2, 4}));
            }

            [Test]
            public void ReturnOnlyPublishedConcerts()
            {
                var concertDetailsService = new RavenConcertDetailsService(Session, GetMockDateTimeProvider());

                var futureConcerts = concertDetailsService.GetFutureConcerts();

                Assert.That(futureConcerts.Select(c => c.Id), Is.Not.Contains(3));
            }

            [Test]
            public void ReturnTheConcertsSortedByDateAscending()
            {
                var concertDetailsService = new RavenConcertDetailsService(Session, GetMockDateTimeProvider());

                var futureConcerts = concertDetailsService.GetFutureConcerts();

                Assert.That(futureConcerts.Select(c => c.Id), Is.EqualTo(new[] { 4, 2 }));
            }

            [SetUp]
            public void CreateSampleData()
            {
                using (var sampleDataSession = Store.OpenSession())
                {
                    sampleDataSession.Store(new Concert(1, "Past Concert", new DateTime(2012, 06, 29), "Venue"));
                    
                    var futurePublishedConcert = new Concert(2, "Future Concert", new DateTime(2013, 03, 08), "Venue");
                    futurePublishedConcert.Publish();
                    sampleDataSession.Store(futurePublishedConcert);
                    
                    sampleDataSession.Store(new Concert(3, "Unpublished Concert", new DateTime(2013, 03, 08), "Venue"));
                    
                    futurePublishedConcert = new Concert(4, "Future Concert", new DateTime(2013, 02, 14), "Venue");
                    futurePublishedConcert.Publish();
                    sampleDataSession.Store(futurePublishedConcert);
                    
                    sampleDataSession.SaveChanges();
                }
            }

            private static IDateTimeProvider GetMockDateTimeProvider()
            {
                var dateTimeProvider = Substitute.For<IDateTimeProvider>();
                dateTimeProvider.Now.Returns(new DateTime(2013, 02, 09, 17, 59, 43));

                return dateTimeProvider;
            }
        }

        [TestFixture]
        public class SaveChangesShould : RavenTest
        {
            [Test]
            public void AllowTheConcertToBeRoundTripped()
            {
                var concert = new Concert(1, "Test Concert", DateTime.MinValue, "Venue");
                var concertDetailsService = new RavenConcertDetailsService(Session, Substitute.For<IDateTimeProvider>());

                concertDetailsService.SaveConcert(concert);
                var actualConcert = Session.Load<Concert>(1);

                Assert.That(actualConcert.Id, Is.EqualTo(concert.Id));
                Assert.That(actualConcert.Title, Is.EqualTo(concert.Title));
                Assert.That(actualConcert.DateAndStartTime, Is.EqualTo(concert.DateAndStartTime));
                Assert.That(actualConcert.Location, Is.EqualTo(concert.Location));
            } 
        }
    }
}
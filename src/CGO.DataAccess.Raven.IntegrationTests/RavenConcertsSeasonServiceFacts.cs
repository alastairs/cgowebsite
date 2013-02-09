using System;
using System.Collections.Generic;
using System.Linq;
using CGO.Domain;
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
                var concertsSeasonService = new RavenConcertsSeasonService(Session);
                
                var actualConcerts = concertsSeasonService.GetConcertsInSeason(2009);

                Assert.That(actualConcerts.Select(c => c.Id), Is.EqualTo(expectedConcerts.Skip(1).Select(c => c.Id)));
            }

            [Test]
            public void ReturnTheConcertsBefore1AugustInTheFollowingYear()
            {
                var expectedConcerts = CreateConcerts();
                var concertsSeasonService = new RavenConcertsSeasonService(Session);

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
                var concertsSeasonService = new RavenConcertsSeasonService(Session);

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
    }
}

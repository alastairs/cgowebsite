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
                var expected2009Concerts = Create2009Concerts();
                var concertsSeasonService = new RavenConcertsSeasonService(Session);
                
                var actual2009Concerts = concertsSeasonService.GetConcertsInSeason(2009);

                Assert.That(actual2009Concerts.Select(c => c.Id), Is.EqualTo(expected2009Concerts.Skip(1).Select(c => c.Id)));
            }

            private IEnumerable<Concert> Create2009Concerts()
            {
                var concerts2009 = new List<Concert>
                {
                    new Concert(1, "Pre-2009 Season Concert", new DateTime(2009, 06, 29), "West Road Concert Hall"), 
                    new Concert(2, "Michaelmas", new DateTime(2009, 11, 13), "West Road Concert Hall"),
                };

                concerts2009.ForEach(c => c.Publish());
                using (var session = Store.OpenSession())
                {
                    concerts2009.ForEach(session.Store);
                    session.SaveChanges();
                }

                return concerts2009;
            }
        }
    }
}

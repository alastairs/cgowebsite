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
            private List<Concert> concerts2009;

            [Test]
            public void ReturnTheConcertsInTheRequestedSeason()
            {
                var concertsSeasonService = new RavenConcertsSeasonService(Session);
                IReadOnlyCollection<Concert> expected2009Concerts = concerts2009.ToArray();

                var actual2009Concerts = concertsSeasonService.GetConcertsInSeason(2009);

                Assert.That(actual2009Concerts.Select(c => c.Id), Is.EqualTo(expected2009Concerts.Select(c => c.Id)));
            }
            
            [SetUp]
            public void CreateTestData()
            {
                concerts2009 = new List<Concert>
                {
                    new Concert(2, "Michaelmas", new DateTime(2009, 11, 13), "West Road Concert Hall"), 
                    new Concert(3, "Lent", new DateTime(2010, 02, 26), "West Road Concert Hall"), 
                    new Concert(4, "Spring", new DateTime(2010, 04, 2), "West Road Concert Hall"), 
                    new Concert(5, "Summer", new DateTime(2010, 06, 25), "West Road Concert Hall")
                };

                concerts2009.ForEach(c => c.Publish());

                using (var sampleDataSession = Store.OpenSession())
                {
                    concerts2009.ForEach(sampleDataSession.Store);
                    sampleDataSession.SaveChanges();
                }
            }
        }
    }
}

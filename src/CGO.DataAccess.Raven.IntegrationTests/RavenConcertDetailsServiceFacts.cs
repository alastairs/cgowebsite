using System;
using CGO.Domain;
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
                var concertDetailService = new RavenConcertDetailsService(Session);
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
    }
}
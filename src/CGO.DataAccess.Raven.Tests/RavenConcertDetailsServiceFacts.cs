using CGO.Domain;
using NSubstitute;
using NUnit.Framework;
using Raven.Client;

namespace CGO.DataAccess.Raven.Tests
{
    public class RavenConcertDetailsServiceFacts
    {
        [TestFixture]
        public class GetConcertShould
        {
            [Test]
            public void CallSessionLoadWithTheProvidedIdentity()
            {
                var ravenSession = Substitute.For<IDocumentSession>();
                var concertDetailsService = new RavenConcertDetailsService(ravenSession);
                const int concertId = 3;

                concertDetailsService.GetConcert(concertId);

                ravenSession.Received().Load<Concert>(concertId);
            }
        }
    }
}

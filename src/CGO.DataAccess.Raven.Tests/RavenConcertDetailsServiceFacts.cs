using System;
using CGO.Domain;
using CGO.Domain.Entities;
using CGO.Domain.Services.Application;
using NSubstitute;
using NUnit.Framework;
using Raven.Client;

namespace CGO.DataAccess.Raven.Tests
{
    public class RavenConcertDetailsServiceFacts
    {
        [TestFixture]
        public class ConstructorShould
        {
            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheRavenSessionIsNull()
            {
                Assert.That(() => new RavenConcertDetailsService(null, Substitute.For<IDateTimeProvider>()),
                            Throws.InstanceOf<ArgumentNullException>());
            }
            
            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheDateTimeProviderIsNull()
            {
                Assert.That(() => new RavenConcertDetailsService(Substitute.For<IDocumentSession>(), null),
                            Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class GetConcertShould
        {
            [Test]
            public void CallSessionLoadWithTheProvidedIdentity()
            {
                var ravenSession = Substitute.For<IDocumentSession>();
                var concertDetailsService = new RavenConcertDetailsService(ravenSession, Substitute.For<IDateTimeProvider>());
                const int concertId = 3;

                concertDetailsService.GetConcert(concertId);

                ravenSession.Received().Load<Concert>(concertId);
            }

            [Test]
            public void ReturnTheResultOfSessionLoad()
            {
                var ravenSession = Substitute.For<IDocumentSession>();
                var expectedConcert = new Concert(1, "Test Concert", DateTime.MinValue, "Venue");
                ravenSession.Load<Concert>(1).Returns(expectedConcert);
                var concertDetailsService = new RavenConcertDetailsService(ravenSession, Substitute.For<IDateTimeProvider>());

                var actualConcert = concertDetailsService.GetConcert(1);

                Assert.That(actualConcert, Is.EqualTo(expectedConcert));
            }
        }

        [TestFixture]
        public class SaveConcertShould
        {
            [Test]
            public void CallStoreOnTheRavenSessionWithTheProvidedConcert()
            {
                var ravenSession = Substitute.For<IDocumentSession>();
                var concertDetailsService = new RavenConcertDetailsService(ravenSession,
                                                                           Substitute.For<IDateTimeProvider>());
                var concert = new Concert(1, "Test Concert", DateTime.MinValue, "Venue");
                
                concertDetailsService.SaveConcert(concert);

                ravenSession.Received(1).Store(concert);
            }

            [Test]
            public void CallSaveChangesOnTheRavenSession()
            {
                var ravenSession = Substitute.For<IDocumentSession>();
                var concertDetailsService = new RavenConcertDetailsService(ravenSession,
                                                                           Substitute.For<IDateTimeProvider>());

                concertDetailsService.SaveConcert(new Concert(1, "Test Concert", DateTime.MinValue, "Venue"));

                ravenSession.Received(1).SaveChanges();
            }
        }
    }
}

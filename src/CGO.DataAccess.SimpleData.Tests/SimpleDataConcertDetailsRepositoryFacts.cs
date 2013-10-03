using CGO.Domain;
using CGO.Domain.Services;
using NUnit.Framework;

namespace CGO.DataAccess.SimpleData.Tests
{
    public class SimpleDataConcertDetailsRepositoryFacts
    {
        [TestFixture]
        public class ItShould
        {
            [Test]
            public void ImplementIConcertDetailsService()
            {
                Assert.That(new SimpleDataConcertDetailsRepository(), Is.AssignableTo<IConcertDetailsService>());
            }
        }
    }
}

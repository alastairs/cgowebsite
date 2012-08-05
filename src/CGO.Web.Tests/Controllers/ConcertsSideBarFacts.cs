using System;
using System.Web.Mvc;
using System.Web.Routing;

using CGO.Web.Controllers;
using CGO.Web.Infrastructure;

using NSubstitute;

using NUnit.Framework;

namespace CGO.Web.Tests.Controllers
{
    class ConcertsSideBarFacts
    {
        [TestFixture]
        public class ConstructorShould
        {
            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheUrlHelperIsNull()
            {
                Assert.That(() => new ConcertsSideBar(null, Substitute.For<IDocumentSessionFactory>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheDocumentSessionFactoryIsNull()
            {
                Assert.That(() => new ConcertsSideBar(Substitute.For<IUrlHelper>(), null), Throws.InstanceOf<ArgumentNullException>());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Web.Controllers;
using CGO.Web.Models;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

using Raven.Client;

namespace CGO.Web.Tests.Controllers
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class IndexShould
        {
            [Test]
            public void DisplayTheIndexView()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSessionFactory>());

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsInDescendingOrderByDate()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSessionFactory>());

                var result = controller.Index() as ViewResult;
                var concerts = result.Model as IEnumerable<Concert>;

                Assert.That(concerts.OrderByDescending(c => c.DateAndStartTime), Is.EqualTo(concerts));
            }
        }

        [TestFixture]
        public class DetailsShould
        {
            [Test]
            public void RenderTheDetailsView()
            {
                var documentSessionFactory = GetMockDocumentSessionFactory();
                var controller = new ConcertsController(documentSessionFactory);

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                var documentSessionFactory = GetMockDocumentSessionFactory();
                var controller = new ConcertsController(documentSessionFactory);

                var result = controller.Details(1) as ViewResult;
                var concert = result.Model as Concert;
                
                Assert.That(concert.Id, Is.EqualTo(1));
            }

            private IDocumentSessionFactory GetMockDocumentSessionFactory()
            {
                var documentSession = Substitute.For<IDocumentSession>();
                documentSession.Load<Concert>(Arg.Any<int>()).Returns(new Concert(1, "foo", DateTime.Now, "bar"));

                var documentSessionFactory = Substitute.For<IDocumentSessionFactory>();
                documentSessionFactory.CreateSession().Returns(documentSession);

                return documentSessionFactory;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Web.Controllers;
using CGO.Web.Models;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

using Raven.Client.Embedded;

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
            private EmbeddableDocumentStore store;

            [Test]
            public void RenderTheDetailsView()
            {
                var documentSessionFactory = GetInMemoryDocumentSessionFactory();
                var controller = new ConcertsController(documentSessionFactory);

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                var documentSessionFactory = GetInMemoryDocumentSessionFactory();
                var controller = new ConcertsController(documentSessionFactory);

                var result = controller.Details(1) as ViewResult;
                var concert = result.Model as Concert;
                
                Assert.That(concert.Id, Is.EqualTo(1));
            }

            [SetUp]
            public void ConfigureRavenDb()
            {
                store = new EmbeddableDocumentStore { RunInMemory = true };
                store.Initialize();
            }

            [SetUp]
            public void CreateSampleData()
            {
                using(var session = store.OpenSession())
                {
                    session.Store(new Concert(1, "foo", DateTime.Now, "bar"));
                    session.SaveChanges();
                }
            }

            private IDocumentSessionFactory GetInMemoryDocumentSessionFactory()
            {
                var documentSessionFactory = Substitute.For<IDocumentSessionFactory>();
                documentSessionFactory.CreateSession().Returns(store.OpenSession());

                return documentSessionFactory;
            }
        }
    }
}

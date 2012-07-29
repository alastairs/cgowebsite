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
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsInDescendingOrderByDate()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Index() as ViewResult;
                var concerts = result.Model as IEnumerable<Concert>;

                Assert.That(concerts.OrderByDescending(c => c.DateAndStartTime), Is.EqualTo(concerts));
            }
        }

        [TestFixture]
        public class DetailsShould
        {
            private static EmbeddableDocumentStore store;
            private IDocumentSession session;

            [Test]
            public void RenderTheDetailsView()
            {
                var controller = new ConcertsController(session);

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                var controller = new ConcertsController(session);

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
                using(var sampleDataSession = store.OpenSession())
                {
                    sampleDataSession.Store(new Concert(1, "foo", DateTime.Now, "bar"));
                    sampleDataSession.SaveChanges();
                }
            }

            [SetUp]
            public void OpenSession()
            {
                session = store.OpenSession();
            }

            [TearDown]
            public void CloseSession()
            {
                session.Dispose();
            }

            [TearDown]
            public void DestroyRavenDbStore()
            {
                store.Dispose();
            }
        }
    }
}

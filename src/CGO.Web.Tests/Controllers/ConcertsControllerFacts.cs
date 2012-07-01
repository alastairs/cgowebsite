using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CGO.Web.Controllers;
using CGO.Web.Models;
using MvcContrib.TestHelper;
using NUnit.Framework;

namespace CGO.Web.Tests
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class IndexShould
        {
            [Test]
            public void DisplayTheIndexView()
            {
                var controller = new ConcertsController();

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsInDescendingOrderByDate()
            {
                var controller = new ConcertsController();

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
                var controller = new ConcertsController();

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                var controller = new ConcertsController();

                var result = controller.Details(1) as ViewResult;
                var concert = result.Model as Concert;
                
                Assert.That(concert.Id, Is.EqualTo(1));
            }
        }
    }
}

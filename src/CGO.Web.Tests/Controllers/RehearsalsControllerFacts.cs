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
    public class RehearsalsControllerFacts
    {
        [TestFixture]
        public class IndexShould
        {
            [Test]
            public void RenderTheIndexView()
            {
                var controller = new RehearsalsController();

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Rehearsal>>();
            }

            [Test]
            public void DisplayTheRehearsalsInAscendingOrderByDate()
            {
                var controller = new RehearsalsController();

                var result = controller.Index() as ViewResult;
                var rehearsals = result.Model as IEnumerable<Rehearsal>;

                Assert.That(rehearsals.OrderBy(r => r.DateAndStartTime), Is.EqualTo(rehearsals));
            }

            [Test]
            public void DisplayOnlyRehearsalsThatTakePlaceInTheFuture()
            {
                var controller = new RehearsalsController();

                var result = controller.Index() as ViewResult;
                var rehearsals = result.Model as IEnumerable<Rehearsal>;

                Assert.That(rehearsals.All(r => r.DateAndStartTime > DateTime.Now));
            }
        }

        [TestFixture]
        public class DetailsShould
        {
            [Test]
            public void RenderTheDetailsView()
            {
                var controller = new RehearsalsController();

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Rehearsal>();
            }

            [Test]
            public void DisplayTheRehearsalRequested()
            {
                var controller = new RehearsalsController();

                var result = controller.Details(1) as ViewResult;
                var rehearsal = result.Model as Rehearsal;

                Assert.That(rehearsal.Id, Is.EqualTo(1));
            }
        }
    }
}

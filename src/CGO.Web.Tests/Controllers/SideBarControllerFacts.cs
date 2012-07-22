using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using CGO.Web.Controllers;
using CGO.Web.Models;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

namespace CGO.Web.Tests.Controllers
{
    public class SideBarControllerFacts
    {
        [TestFixture]
        public class ConstructorShould
        {
            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheSideBarProviderFactoryArgumentIsNull()
            {
                Assert.That(() => new SideBarController(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class DisplayShould
        {
            [Test]
            public void RenderTheSideBarPartialViewIfThereAreLinksToDisplay()
            {
                var urlHelper = Substitute.For<UrlHelper>(Substitute.For<RequestContext>());
                var sideBarProvider = Substitute.For<SideBar>(urlHelper);
                sideBarProvider.GetSideBarSections().Returns(_ => new List<SideBarSection>{ new SideBarSection("foo", Enumerable.Empty<SideBarLink>())});
                var controller = new SideBarController(sideBarProvider);

                var result = controller.Display();

                result.AssertPartialViewRendered().ForView("_Sidebar");
            }

            [Test]
            public void RenderTheSideBarPartialViewWithTheAppropriateLinks()
            {
                var urlHelper = Substitute.For<UrlHelper>(Substitute.For<RequestContext>());
                var sideBarProvider = Substitute.For<SideBar>(urlHelper);
                var expected = new List<SideBarSection> {new SideBarSection("foo", Enumerable.Empty<SideBarLink>())};
                sideBarProvider.GetSideBarSections().Returns(_ => expected);
                var controller = new SideBarController(sideBarProvider);

                var result = controller.Display();

                var renderedData = result.AssertPartialViewRendered().WithViewData<IEnumerable<SideBarSection>>();
                Assert.That(renderedData, Is.EqualTo(expected));
            }

            [Test]
            public void NotRenderTheSideBarIfThereAreNoLinksToDisplay()
            {
                var controller = new SideBarController(Substitute.For<SideBar>(Substitute.For<UrlHelper>(Substitute.For<RequestContext>())));

                var result = controller.Display();

                Assert.That(result, Is.Null);
            }
        }
    }
}

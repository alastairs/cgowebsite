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
                var sideBarProvider = Substitute.For<SideBarProvider>(urlHelper);
                sideBarProvider.GetSideBarSections().Returns(_ => new List<SideBarSection>{ new SideBarSection("foo", Enumerable.Empty<SideBarLink>())});
                var controller = new SideBarController(sideBarProvider);

                var result = controller.Display();

                result.AssertPartialViewRendered();
            }

            [Test]
            public void ReturnNotRenderTheSideBarIfThereAreNoLinksToDisplay()
            {
                var controller = new SideBarController(Substitute.For<SideBarProvider>(Substitute.For<UrlHelper>(Substitute.For<RequestContext>())));

                var result = controller.Display();

                Assert.That(result, Is.Null);
            }
        }
    }
}

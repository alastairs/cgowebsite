using System;
using System.Collections.Generic;
using System.Linq;
using CGO.Web.Controllers;
using CGO.Web.Infrastructure;
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
                Assert.That(() => new SideBarController(null, Substitute.For<IDocumentSessionFactory>()), Throws.InstanceOf<ArgumentNullException>());
            }

            [Test]
            public void ThrowAnArgumentNullExceptionWhenTheDocumentSessionFactoryArgumentIsNull()
            {
                Assert.That(() => new SideBarController(Substitute.For<ISideBarFactory>(), null), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class DisplayShould
        {
            [Test]
            public void RenderTheSideBarPartialViewIfThereAreLinksToDisplay()
            {
                var expectedSideBarSections = new List<SideBarSection> {new SideBarSection("foo", Enumerable.Empty<SideBarLink>())};
                var controller = new SideBarController(GetMockSideBarFactory(expectedSideBarSections), Substitute.For<IDocumentSessionFactory>());
                new TestControllerBuilder().InitializeController(controller);
                
                var result = controller.Display();

                result.AssertPartialViewRendered().ForView("_Sidebar");
            }

            [Test]
            public void RenderTheSideBarPartialViewWithTheAppropriateLinks()
            {
                var expectedSideBarSections = new List<SideBarSection> { new SideBarSection("foo", Enumerable.Empty<SideBarLink>()) };
                var controller = new SideBarController(GetMockSideBarFactory(expectedSideBarSections), Substitute.For<IDocumentSessionFactory>());
                new TestControllerBuilder().InitializeController(controller);

                var result = controller.Display();

                var renderedData = result.AssertPartialViewRendered().WithViewData<IEnumerable<SideBarSection>>();
                Assert.That(renderedData, Is.EqualTo(expectedSideBarSections));
            }

            [Test]
            public void NotRenderTheSideBarIfThereAreNoLinksToDisplay()
            {
                var controller = new SideBarController(GetMockSideBarFactory(Enumerable.Empty<SideBarSection>()), Substitute.For<IDocumentSessionFactory>());
                new TestControllerBuilder().InitializeController(controller);

                var result = controller.Display();

                Assert.That(result, Is.Null);
            }

            private static ISideBarFactory GetMockSideBarFactory(IEnumerable<SideBarSection> expectedSideBarSections)
            {
                var sideBar = Substitute.For<SideBar>(Substitute.For<IUrlHelper>());
                sideBar.GetSideBarSections().Returns(_ => expectedSideBarSections); 
                
                var sideBarFactory = Substitute.For<ISideBarFactory>();
                sideBarFactory.CreateSideBar(null, null).ReturnsForAnyArgs(sideBar);
                
                return sideBarFactory;
            }
        }
    }
}

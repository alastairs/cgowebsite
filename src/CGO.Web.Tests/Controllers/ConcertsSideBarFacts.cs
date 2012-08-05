using System;
using System.ComponentModel;
using System.Linq;

using CGO.Web.Controllers;
using CGO.Web.Infrastructure;
using CGO.Web.Models;
using CGO.Web.Tests.EqualityComparers;

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

        [TestFixture]
        public class GetSideBarSectionsShould : RavenTest
        {
            private SideBarSection currentSeason;

            [Test]
            public void ReturnTheFirstSectionForTheCurrentSeason()
            {
                var sideBar = new ConcertsSideBar(GetMockUrlHelper(), new DocumentSessionFactory(Store));

                var sections = sideBar.GetSideBarSections();

                Assert.That(sections.First().Title, Is.EqualTo(currentSeason.Title));
            }

            [SetUp]
            public void CreateExpectedSideBarSections()
            {
                currentSeason = new SideBarSection("Current Season", new[]
                {
                    new SideBarLink("Concert 1", "/Concerts/Details/1", false),
                    new SideBarLink("Concert 2", "/Concerts/Details/2", false),
                    new SideBarLink("Concert 3", "/Concerts/Details/3", false),
                    new SideBarLink("Concert 4", "/Concerts/Details/4", false)
                });
            }

            [SetUp]
            public void CreateSampleData()
            {
                var concerts = new[]
                {
                    new Concert(1, "Concert 1", DateTime.Now.AddDays(1d), "Concert Hall"),
                    new Concert(3, "Concert 3", DateTime.Now.AddDays(3d), "Concert Hall"),
                    new Concert(2, "Concert 2", DateTime.Now.AddDays(2d), "Concert Hall"),
                    new Concert(4, "Concert 4", DateTime.Now.AddDays(4d), "Concert Hall")
                };

                using (var sampleDataSession = Store.OpenSession())
                {
                    foreach (var concert in concerts)
                    {
                        sampleDataSession.Store(concert);
                    }

                    sampleDataSession.SaveChanges();
                }
            }
        }

        private static IUrlHelper GetMockUrlHelper()
        {
            var mockUrlHelper = Substitute.For<IUrlHelper>();
            mockUrlHelper.Action(string.Empty, string.Empty, null)
                         .ReturnsForAnyArgs(callInfo => string.Format("/{1}/{0}/{2}", callInfo.Args()[0],
                                                                      callInfo.Args()[1], GetIdFromObject(callInfo.Args()[2])));

            return mockUrlHelper;
        }

        private static object GetIdFromObject(object arg)
        {
            var properties = TypeDescriptor.GetProperties(arg);

            return properties.Cast<PropertyDescriptor>()
                             .Where(property => property.Name == "id")
                             .Select(property => property.GetValue(arg))
                             .FirstOrDefault();
        }
    }
}

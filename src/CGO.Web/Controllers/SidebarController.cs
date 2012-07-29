using System;
using System.Linq;
using System.Web.Mvc;

namespace CGO.Web.Controllers
{
    public class SideBarController : Controller
    {
        private readonly ISideBarFactory sideBarFactory;
        private readonly IDocumentSessionFactory documentSessionFactory;

        public SideBarController(ISideBarFactory sideBarFactory, IDocumentSessionFactory documentSessionFactory)
        {
            if (sideBarFactory == null)
            {
                throw new ArgumentNullException("sideBarFactory");
            }

            if (documentSessionFactory == null)
            {
                throw new ArgumentNullException("documentSessionFactory");
            }

            this.sideBarFactory = sideBarFactory;
            this.documentSessionFactory = documentSessionFactory;
        }

        [ChildActionOnly]
        public virtual ActionResult Display()
        {
            var sideBar = sideBarFactory.CreateSideBar(Url, ControllerContext.RouteData.Values["RequestingController"] as string, documentSessionFactory);
            var sideBarSections = sideBar.GetSideBarSections();

            return sideBarSections.Any() ? PartialView("_Sidebar", sideBarSections) : null;
        }
    }
}
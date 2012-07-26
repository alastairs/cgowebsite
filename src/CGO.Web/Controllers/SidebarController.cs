using System;
using System.Linq;
using System.Web.Mvc;

namespace CGO.Web.Controllers
{
    public class SideBarController : Controller
    {
        private readonly ISideBarFactory sideBarFactory;

        public SideBarController(ISideBarFactory sideBarFactory)
        {
            if (sideBarFactory == null)
            {
                throw new ArgumentNullException("sideBarFactory");
            }

            this.sideBarFactory = sideBarFactory;
        }

        [ChildActionOnly]
        public virtual ActionResult Display()
        {
            var sideBar = sideBarFactory.CreateSideBar(Url, ControllerContext.RouteData.Values["RequestingController"] as string);
            var sideBarSections = sideBar.GetSideBarSections();

            return sideBarSections.Any() ? PartialView("_Sidebar", sideBarSections) : null;
        }
    }
}
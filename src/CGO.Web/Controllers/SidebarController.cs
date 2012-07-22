using System;
using System.Linq;
using System.Web.Mvc;

namespace CGO.Web.Controllers
{
    public class SideBarController : Controller
    {
        private readonly SideBar sideBar;

        public SideBarController(SideBar sideBar)
        {
            if (sideBar == null)
            {
                throw new ArgumentNullException("sideBar");
            }

            this.sideBar = sideBar;
        }

        [ChildActionOnly]
        public virtual ActionResult Display()
        {
            var sideBarSections = sideBar.GetSideBarSections();

            return sideBarSections.Any() ? PartialView("_Sidebar", sideBarSections) : null;
        }
    }
}
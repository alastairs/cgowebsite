using System;
using System.Linq;
using System.Web.Mvc;

namespace CGO.Web.Controllers
{
    public class SideBarController : Controller
    {
        private readonly SideBarProvider sideBarProvider;

        public SideBarController(SideBarProvider sideBarProvider)
        {
            if (sideBarProvider == null)
            {
                throw new ArgumentNullException("sideBarProvider");
            }

            this.sideBarProvider = sideBarProvider;
        }

        [ChildActionOnly]
        public virtual ActionResult Display()
        {
            var sideBarSections = sideBarProvider.GetSideBarSections();

            return sideBarSections.Any() ? PartialView("_Sidebar", sideBarSections) : null;
        }
    }
}
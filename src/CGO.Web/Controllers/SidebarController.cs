using System;
using System.Linq;
using System.Web.Mvc;

using CGO.Web.Infrastructure;

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
            var sideBar = sideBarFactory.CreateSideBar(new MvcUrlHelper(Url));
            var sideBarSections = sideBar.GetSideBarSections();

            return sideBarSections.Any() ? PartialView("_Sidebar", sideBarSections) : null;
        }
    }
}
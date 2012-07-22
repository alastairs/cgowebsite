using System.Collections.Generic;
using System.Web.Mvc;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public abstract class SideBar
    {
        protected SideBar(UrlHelper urlHelper)
        {
            Url = urlHelper;
        }

        protected UrlHelper Url { get; private set; }

        public abstract IEnumerable<SideBarSection> GetSideBarSections();
    }
}
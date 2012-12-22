using System;
using System.Collections.Generic;

using CGO.Web.Infrastructure;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public abstract class SideBar
    {
        protected SideBar(IUrlHelper urlHelper)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException("urlHelper");
            }

            Url = urlHelper;
        }

        protected IUrlHelper Url { get; private set; }

        public abstract IEnumerable<SideBarSection> GetSideBarSections();
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class EmptySideBar : SideBar
    {
        public EmptySideBar(UrlHelper urlHelper) : base(urlHelper) { }
        public override IEnumerable<SideBarSection> GetSideBarSections()
        {
            return Enumerable.Empty<SideBarSection>();
        }
    }
}
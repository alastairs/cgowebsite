using System.Collections.Generic;
using System.Linq;

using CGO.Web.Infrastructure;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class EmptySideBar : SideBar
    {
        public EmptySideBar(IUrlHelper urlHelper) : base(urlHelper) { }
        public override IEnumerable<SideBarSection> GetSideBarSections()
        {
            return Enumerable.Empty<SideBarSection>();
        }
    }
}
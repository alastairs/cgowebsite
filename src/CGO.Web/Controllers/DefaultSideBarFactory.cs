using System;
using System.Collections.Generic;
using System.Linq;
using CGO.Web.Infrastructure;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class DefaultSideBarFactory : ISideBarFactory
    {
        public SideBar CreateSideBar(IUrlHelper urlHelper, IDocumentSessionFactory documentSessionFactory)
        {
            return new DefaultSideBar(urlHelper);
        }
    }

    public class DefaultSideBar : SideBar
    {
        public DefaultSideBar(IUrlHelper urlHelper) : base(urlHelper) { }
        public override IEnumerable<SideBarSection> GetSideBarSections()
        {
            return Enumerable.Empty<SideBarSection>();
        }
    }
}
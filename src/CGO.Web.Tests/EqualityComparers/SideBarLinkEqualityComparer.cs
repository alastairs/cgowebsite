using System.Collections.Generic;

using CGO.Web.Models;

namespace CGO.Web.Tests.EqualityComparers
{
    internal class SideBarLinkEqualityComparer : IEqualityComparer<SideBarLink> 
    {
        public bool Equals(SideBarLink x, SideBarLink y)
        {
            var titlesAreEqual = x.Title == y.Title;
            var urisAreEqual = x.Uri == y.Uri;
            var isActivesAreEqual = x.IsActive == y.IsActive;

            return titlesAreEqual && urisAreEqual && isActivesAreEqual;
        }

        public int GetHashCode(SideBarLink obj)
        {
            return obj.Title.GetHashCode();
        }
    }
}
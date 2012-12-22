using System.Collections.Generic;
using System.Linq;

using CGO.Web.Models;

namespace CGO.Web.Tests.EqualityComparers
{
    /// <summary>
    /// Determines whether or not two SideBarSections are equal.  To be equal, the SideBarSections must have 
    /// the same title, the same number of links, and the same links in the same order.  
    /// </summary>
    internal class SideBarSectionEqualityComparer : IEqualityComparer<SideBarSection>
    {
        public bool Equals(SideBarSection x, SideBarSection y)
        {
            bool titlesAreEqual = x.Title == y.Title;
            bool linkLengthsAreEqual = x.Links.Count() == y.Links.Count();

            if (!titlesAreEqual || !linkLengthsAreEqual)
            {
                return false;
            }

            bool linksAreEqual = true; // Seed with true to ensure the logic works
            for (int i = 0; i < x.Links.Count(); i++)
            {
                linksAreEqual = linksAreEqual && new SideBarLinkEqualityComparer().Equals(x.Links.ElementAt(i), y.Links.ElementAt(i));
            }

            return linksAreEqual;
        }

        public int GetHashCode(SideBarSection obj)
        {
            return obj.Title.GetHashCode();
        }
    }
}
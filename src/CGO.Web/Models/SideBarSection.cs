using System.Collections.Generic;

namespace CGO.Web.Models
{
    public class SideBarSection
    {
        public SideBarSection(string title, IEnumerable<SideBarLink> links)
        {
            Title = title;
            Links = links;
        }

        public string Title { get; private set; }
        public IEnumerable<SideBarLink> Links { get; private set; }
    }
}
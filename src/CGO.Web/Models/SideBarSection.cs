using System.Collections.Generic;
using System.Linq;

namespace CGO.Web.Models
{
    public class SideBarSection
    {
        private readonly ICollection<SideBarLink> links;

        public SideBarSection(string title, IEnumerable<SideBarLink> links)
        {
            Title = title;
            this.links = links.ToList();
        }

        public SideBarSection(string title) : this(title, Enumerable.Empty<SideBarLink>()){}

        public string Title { get; private set; }
        public IEnumerable<SideBarLink> Links { get { return links; } }

        public void AddLink(SideBarLink sideBarLink)
        {
            links.Add(sideBarLink);
        }
    }
}
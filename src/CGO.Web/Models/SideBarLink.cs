namespace CGO.Web.Models
{
    public class SideBarLink
    {
        public SideBarLink(string title, string uri, bool isActive)
        {
            Uri = uri;
            Title = title;
            IsActive = isActive;
        }

        public string Title { get; private set; }
        public string Uri { get; private set; }
        public bool IsActive { get; private set; }
    }
}
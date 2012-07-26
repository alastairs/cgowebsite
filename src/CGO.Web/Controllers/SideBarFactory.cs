using System.Web.Mvc;

namespace CGO.Web.Controllers
{
    public class SideBarFactory : ISideBarFactory
    {
        public SideBar CreateSideBar(UrlHelper urlHelper, string controllerName)
        {
            switch(controllerName)
            {
                case "Concerts":
                    return new ConcertsSideBar(urlHelper);
                default:
                    return new EmptySideBar(urlHelper);
            }
        }
    }
}
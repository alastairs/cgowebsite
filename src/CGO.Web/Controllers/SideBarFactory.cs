using CGO.Web.Infrastructure;

namespace CGO.Web.Controllers
{
    public class SideBarFactory : ISideBarFactory
    {
        public SideBar CreateSideBar(IUrlHelper urlHelper, string controllerName, IDocumentSessionFactory documentSessionFactory)
        {
            switch(controllerName)
            {
                case "Concerts":
                    return new ConcertsSideBar(urlHelper, documentSessionFactory);
                default:
                    return new EmptySideBar(urlHelper);
            }
        }
    }
}
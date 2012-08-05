using System;

using CGO.Web.Infrastructure;

namespace CGO.Web.Controllers
{
    public class SideBarFactory : ISideBarFactory
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public SideBarFactory(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            this.dateTimeProvider = dateTimeProvider;
        }

        public SideBar CreateSideBar(IUrlHelper urlHelper, string controllerName, IDocumentSessionFactory documentSessionFactory)
        {
            switch(controllerName)
            {
                case "Concerts":
                    return new ConcertsSideBar(urlHelper, documentSessionFactory, dateTimeProvider);
                default:
                    return new EmptySideBar(urlHelper);
            }
        }
    }
}
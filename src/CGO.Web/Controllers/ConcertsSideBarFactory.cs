using System;
using CGO.Domain;
using CGO.Web.Infrastructure;

namespace CGO.Web.Controllers
{
    public class ConcertsSideBarFactory : ISideBarFactory
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public ConcertsSideBarFactory(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            this.dateTimeProvider = dateTimeProvider;
        }

        public SideBar CreateSideBar(IUrlHelper urlHelper, IDocumentSessionFactory documentSessionFactory)
        {
            return new ConcertsSideBar(urlHelper, documentSessionFactory, dateTimeProvider);
        }
    }
}
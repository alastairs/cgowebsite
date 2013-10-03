using System;
using CGO.Domain;
using CGO.Domain.Services;
using CGO.Domain.Services.Application;
using CGO.Web.Infrastructure;

namespace CGO.Web.Controllers
{
    public class ConcertsSideBarFactory : ISideBarFactory
    {
        private readonly IConcertsSeasonService concertsSeasonService;
        private readonly IDateTimeProvider dateTimeProvider;

        public ConcertsSideBarFactory(IConcertsSeasonService concertsSeasonService, IDateTimeProvider dateTimeProvider)
        {
            if (concertsSeasonService == null)
            {
                throw new ArgumentNullException("concertsSeasonService");
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            this.concertsSeasonService = concertsSeasonService;
            this.dateTimeProvider = dateTimeProvider;
        }

        public SideBar CreateSideBar(IUrlHelper urlHelper)
        {
            return new ConcertsSideBar(urlHelper, concertsSeasonService, dateTimeProvider);
        }
    }
}
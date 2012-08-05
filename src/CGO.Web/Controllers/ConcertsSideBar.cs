using System;
using System.Collections.Generic;
using System.Linq;

using CGO.Web.Infrastructure;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class ConcertsSideBar : SideBar
    {
        private readonly IDocumentSessionFactory documentSessionFactory;
        private readonly IDateTimeProvider dateTimeProvider;

        public ConcertsSideBar(IUrlHelper urlHelper, IDocumentSessionFactory documentSessionFactory, IDateTimeProvider dateTimeProvider) : base(urlHelper)
        {
            if (documentSessionFactory == null)
            {
                throw new ArgumentNullException("documentSessionFactory");
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            this.documentSessionFactory = documentSessionFactory;
            this.dateTimeProvider = dateTimeProvider;
        }

        public override IEnumerable<SideBarSection> GetSideBarSections()
        {
            var currentSeason = new SideBarSection("Current Season");

            using(var session = documentSessionFactory.CreateSession())
            {
                var concerts = session.Query<Concert>().OrderBy(c => c.DateAndStartTime).ToList().Where(ConcertIsInCurrentSeason);
                foreach (var concert in concerts)
                {
                    currentSeason.AddLink(new SideBarLink(concert.Title, Url.Action("Details", "Concerts", new { id = concert.Id }), false));
                }
            }

            return new[] { currentSeason };
        }

        private bool ConcertIsInCurrentSeason(Concert concert)
        {
            var currentYear = dateTimeProvider.Now.Month > 7 ? dateTimeProvider.Now.Year : dateTimeProvider.Now.Year - 1;
            
            return concert.DateAndStartTime >= new DateTime(currentYear, 08, 01) && concert.DateAndStartTime <= new DateTime(currentYear + 1, 07, 31);
        }
    }
}
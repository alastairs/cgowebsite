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
            using(var session = documentSessionFactory.CreateSession())
            {
                var concerts = session.Query<Concert>()
                                      .OrderBy(c => c.DateAndStartTime)
                                      .ToList(); 
                
                var currentSeason = GetSeasonSideBarSection(concerts, "Current Season", dateTimeProvider.Now.Year);
                var lastSeason = GetSeasonSideBarSection(concerts, "Last Season", dateTimeProvider.Now.Year - 1);

                return new[] { currentSeason, lastSeason };
            }
        }

        private SideBarSection GetSeasonSideBarSection(IEnumerable<Concert> concerts, string title, int year)
        {
            var currentSeason = new SideBarSection(title);

            foreach (var concert in concerts.Where(c => ConcertIsInSeason(c, year)))
            {
                currentSeason.AddLink(new SideBarLink(concert.Title, Url.Action("Details", "Concerts", new { id = concert.Id }), false));
            }

            return currentSeason;
        }

        private bool ConcertIsInSeason(Concert concert, int seasonYear)
        {
            var currentYear = dateTimeProvider.Now.Month > 7 ? seasonYear : seasonYear - 1;
            
            return concert.DateAndStartTime >= new DateTime(currentYear, 08, 01) && concert.DateAndStartTime <= new DateTime(currentYear + 1, 07, 31);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CGO.Domain;
using CGO.Web.Infrastructure;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class ConcertsSideBar : SideBar
    {
        private readonly IConcertsSeasonService concertsSeasonService;
        private readonly IDateTimeProvider dateTimeProvider;

        public ConcertsSideBar(IUrlHelper urlHelper, IConcertsSeasonService concertsSeasonService, IDateTimeProvider dateTimeProvider) : base(urlHelper)
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

        public override IEnumerable<SideBarSection> GetSideBarSections()
        {
            var currentSeason = GetSeasonSideBarSection(concertsSeasonService.GetConcertsInCurrentSeason(),
                                                        "Current Season");
            if (currentSeason.Links.Any())
            {
                yield return currentSeason;
            }

            var lastSeason = GetSeasonSideBarSection(concertsSeasonService.GetConcertsInPreviousSeason(), "Last Season");
            if (lastSeason.Links.Any())
            {
                yield return lastSeason;
            }

            var archiveSection = GetArchiveSection();
            if (archiveSection.Links.Any())
            {
                yield return archiveSection;
            }
        }

        private SideBarSection GetArchiveSection()
        {
            var archiveSection = new SideBarSection("Older");

            int currentArchiveYear = dateTimeProvider.Now.Year - 2;
            while (currentArchiveYear >= Concert.DateOfFirstConcert.Year)
            {
                currentArchiveYear--;
                var archiveSeasonEndYear = currentArchiveYear + 1 - 2000; // Drop the century
                var seasonYears = string.Format("{0}-{1:D2}", currentArchiveYear, archiveSeasonEndYear);
                archiveSection.AddLink(new SideBarLink(string.Format("{0} Season", seasonYears), Url.Action("Archive", "Concerts", new { year = currentArchiveYear }), false));
            }
            
            return archiveSection;
        }

        private SideBarSection GetSeasonSideBarSection(IEnumerable<Concert> concerts, string title)
        {
            var currentSeason = new SideBarSection(title);

            foreach (var concert in concerts)
            {
                currentSeason.AddLink(new SideBarLink(concert.Title, Url.Action("Details", "Concerts", new { id = concert.Id }), false));
            }

            return currentSeason;
        }
    }
}
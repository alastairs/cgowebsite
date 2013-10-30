using System;
using System.Collections.Generic;
using System.Linq;
using CGO.Domain;
using Raven.Client;

namespace CGO.DataAccess.Raven
{
    public class RavenConcertsSeasonService : IConcertsSeasonService
    {
        private readonly IDocumentSession ravenSession;
        private readonly IDateTimeProvider dateTimeProvider;

        public RavenConcertsSeasonService(IDocumentSession ravenSession, IDateTimeProvider dateTimeProvider)
        {
            if (ravenSession == null)
            {
                throw new ArgumentNullException("ravenSession");
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            this.ravenSession = ravenSession;
            this.dateTimeProvider = dateTimeProvider;
        }

        public IReadOnlyCollection<Concert> GetConcertsInCurrentSeason()
        {
            if (StartingNewAcademicYear())
            {
                return GetConcertsInSeason(dateTimeProvider.Now.Year);
            }

            return GetConcertsInSeason(dateTimeProvider.Now.Year - 1);
        }

        public IReadOnlyCollection<Concert> GetConcertsInPreviousSeason()
        {
            return GetConcertsInSeason(dateTimeProvider.Now.Year - 1);
        }

        private bool StartingNewAcademicYear()
        {
            const int academicYearStartMonth = 9;
            return dateTimeProvider.Now.Month >= academicYearStartMonth;
        }

        public IReadOnlyCollection<Concert> GetConcertsInSeason(int seasonStartYear)
        {
            return ravenSession.Query<Concert>()
                               .Where(c => c.DateAndStartTime >= DateTime.Parse(seasonStartYear + "-09-01"))
                               .Where(c => c.DateAndStartTime <= DateTime.Parse(seasonStartYear + 1 + "-08-31"))
                               .Where(c => c.IsPublished)
                               .ToList();
        }
    }
}

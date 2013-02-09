using System;
using System.Collections.Generic;
using System.Linq;
using CGO.Domain;
using Raven.Client;

namespace CGO.DataAccess.Raven
{
    public class RavenConcertDetailsService : IConcertDetailsService
    {
        private readonly IDocumentSession ravenSession;
        private readonly IDateTimeProvider dateTimeProvider;

        public RavenConcertDetailsService(IDocumentSession ravenSession, IDateTimeProvider dateTimeProvider)
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

        public Concert GetConcert(int concertId)
        {
            return ravenSession.Load<Concert>(concertId);
        }

        public IReadOnlyCollection<Concert> GetFutureConcerts()
        {
            return ravenSession.Query<Concert>()
                               .Where(c => c.DateAndStartTime > dateTimeProvider.Now)
                               .Where(c => c.IsPublished)
                               .OrderBy(c => c.DateAndStartTime)
                               .ToList();
        }
    }
}

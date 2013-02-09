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

        public RavenConcertsSeasonService(IDocumentSession ravenSession)
        {
            if (ravenSession == null)
            {
                throw new ArgumentNullException("ravenSession");
            }

            this.ravenSession = ravenSession;
        }

        public IReadOnlyCollection<Concert> GetConcertsInCurrentSeason()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Concert> GetConcertsInPreviousSeason()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Concert> GetConcertsInSeason(int seasonStartYear)
        {
            return ravenSession.Query<Concert>().ToList();
        }
    }
}

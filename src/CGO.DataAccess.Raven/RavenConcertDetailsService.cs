using System;
using CGO.Domain;
using Raven.Client;

namespace CGO.DataAccess.Raven
{
    public class RavenConcertDetailsService : IConcertDetailsService
    {
        private readonly IDocumentSession ravenSession;

        public RavenConcertDetailsService(IDocumentSession ravenSession)
        {
            if (ravenSession == null)
            {
                throw new ArgumentNullException("ravenSession");
            }

            this.ravenSession = ravenSession;
        }

        public Concert GetConcert(int concertId)
        {
            return ravenSession.Load<Concert>(concertId);
        }
    }
}

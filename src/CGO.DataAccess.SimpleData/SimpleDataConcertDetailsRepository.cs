using System.Collections.Generic;
using CGO.Domain;

namespace CGO.DataAccess.SimpleData
{
    public class SimpleDataConcertDetailsRepository : IConcertDetailsService
    {
        public Concert GetConcert(int concertId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<Concert> GetFutureConcerts()
        {
            throw new System.NotImplementedException();
        }

        public void SaveConcert(Concert concert)
        {
            throw new System.NotImplementedException();
        }
    }
}
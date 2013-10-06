using System.Collections.Generic;
using System.Linq;
using CGO.Web.Models;

namespace CGO.Web.Tests.EqualityComparers
{
    public class ConcertsIndexViewModelEqualityComparer : IEqualityComparer<ConcertsIndexViewModel>
    {
        private readonly ConcertViewModelEqualityComparer concertViewModelEqualityComparer = new ConcertViewModelEqualityComparer();

        public bool Equals(ConcertsIndexViewModel x, ConcertsIndexViewModel y)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            if (ReferenceEquals(x.UpcomingConcerts, null) || ReferenceEquals(y.UpcomingConcerts, null))
            {
                return false;
            }

            if (x.UpcomingConcerts.Count() != y.UpcomingConcerts.Count())
            {
                return false;
            }

            var nextConcertsAreEqual = concertViewModelEqualityComparer.Equals(x.NextConcert, y.NextConcert);
            var upcomingConcertsAreEqual = x.UpcomingConcerts.Intersect(y.UpcomingConcerts,
                                                                        concertViewModelEqualityComparer)
                                                             .Count() == x.UpcomingConcerts.Count();

            return nextConcertsAreEqual && upcomingConcertsAreEqual;
        }

        public int GetHashCode(ConcertsIndexViewModel obj)
        {
            return 1;
        }
    }
}
using System.Collections.Generic;

using CGO.Web.ViewModels.Api;

namespace CGO.Web.Tests.EqualityComparers
{
    public class ConcertApiViewModelEqualityComparer : IEqualityComparer<ConcertViewModel>
    {
        public bool Equals(ConcertViewModel x, ConcertViewModel y)
        {
            var titlesAreEqual = x.Title == y.Title;
            var datesAreEqual = x.Date == y.Date;
            var startTimesAreEqual = x.StartTime == y.StartTime;
            var locationsAreEqual = x.Location == y.Location;

            return titlesAreEqual && datesAreEqual && startTimesAreEqual && locationsAreEqual;
        }

        public int GetHashCode(ConcertViewModel concert)
        {
            return concert.Id.GetHashCode();
        }
    }
}
using System.Collections.Generic;
using CGO.Domain;
using CGO.Domain.Entities;
using CGO.Web.Models;

namespace CGO.Web.Tests.EqualityComparers
{
    public class ConcertEqualityComparer : IEqualityComparer<Concert> 
    {
        public bool Equals(Concert x, Concert y)
        {
            var titlesAreEqual = x.Title == y.Title;
            var datesAreEqual = x.DateAndStartTime == y.DateAndStartTime;
            var locationsAreEqual = x.Location == y.Location;

            return titlesAreEqual && datesAreEqual && locationsAreEqual;
        }

        public int GetHashCode(Concert concert)
        {
            return concert.Id.GetHashCode();
        }
    }
}
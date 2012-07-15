using System.Collections.Generic;

namespace CGO.Web.Models
{
    public class HomePageViewModel
    {
        public HomePageViewModel(Concert nextConcert, IEnumerable<Rehearsal> upcomingRehearsals)
        {
            NextConcert = nextConcert;
            UpcomingRehearsals = upcomingRehearsals;
        }

        public Concert NextConcert { get; private set; }
        public IEnumerable<Rehearsal> UpcomingRehearsals { get; private set; }
    }
}
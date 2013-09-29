using System.Collections.Generic;
using System.Linq;
using CGO.Domain;

namespace CGO.Web.Models
{
    public class HomePageViewModel
    {
        public HomePageViewModel(Concert nextConcert, IEnumerable<Rehearsal> upcomingRehearsals)
        {
            NextConcert = nextConcert;

            var rehearsals = upcomingRehearsals.ToList();
            NextRehearsal = rehearsals.First();
            UpcomingRehearsals = rehearsals.Skip(1);
        }

        public Rehearsal NextRehearsal { get; set; }
        public Concert NextConcert { get; private set; }
        public IEnumerable<Rehearsal> UpcomingRehearsals { get; private set; }
    }
}
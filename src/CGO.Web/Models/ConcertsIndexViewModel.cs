using System.Collections.Generic;
using CGO.Web.ViewModels;

namespace CGO.Web.Models
{
    public class ConcertsIndexViewModel
    {
        public ConcertViewModel NextConcert { get; set; }
        public IEnumerable<ConcertViewModel> UpcomingConcerts { get; set; }
    }
}
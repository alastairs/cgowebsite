using System;
using System.Linq;
using System.Web.Mvc;
using CGO.Domain;

namespace CGO.Web.Controllers
{
    public class ConcertsController : Controller
    {
        private readonly IConcertDetailsService concertDetailsService;
        private readonly IConcertsSeasonService concertsSeasonService;

        public ConcertsController(IConcertDetailsService concertDetailsService, IConcertsSeasonService concertsSeasonService)
        {
            if (concertDetailsService == null)
            {
                throw new ArgumentNullException("concertDetailsService");
            }

            if (concertsSeasonService == null)
            {
                throw new ArgumentNullException("concertsSeasonService");
            }

            this.concertDetailsService = concertDetailsService;
            this.concertsSeasonService = concertsSeasonService;
        }

        //
        // GET: /Concerts/

        public ActionResult Index()
        {
            var concerts = concertDetailsService.GetFutureConcerts();

            if (concerts.Any())
            {
                return View("Index", concerts);
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Concerts/Details/5

        public ActionResult Details(int id)
        {
            var concert = concertDetailsService.GetConcert(id);

            if (concert == null)
            {
                return new HttpNotFoundResult();
            }

            return View("Details", concert);
        }

        //
        // GET: /Concerts/Archive/2009

        public ActionResult Archive(int year)
        {
            var concerts = concertsSeasonService.GetConcertsInSeason(year);

            ViewBag.ConcertSeason = string.Format("{0}-{1}", year, year + 1);

            return View("Archive", concerts);
        }

        //
        // GET: /Concerts/Archived/

        public ActionResult Archived()
        {
            return View("ArchiveIndex");
        }
    }
}

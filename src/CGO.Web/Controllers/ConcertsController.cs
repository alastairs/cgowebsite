using System;
using System.Linq;
using System.Web.Mvc;
using CGO.Web.Mappers;
using CGO.Web.Models;
using CGO.Web.ViewModels;

using Raven.Client;

namespace CGO.Web.Controllers
{
    public class ConcertsController : Controller
    {
        private readonly IDocumentSession session;

        public ConcertsController(IDocumentSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            this.session = session;
        }

        //
        // GET: /Concerts/

        public ActionResult Index()
        {
            var concerts = session.Query<Concert>()
                                  .Where(c => c.DateAndStartTime > DateTime.Now)
                                  .Where(c => c.IsPublished)
                                  .OrderBy(c => c.DateAndStartTime)
                                  .ToList();

            if (concerts.Any())
            {
                return View("Index", concerts);
            }

            if (Request.IsAuthenticated)
            {
                return View("List");
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Concerts/Details/5

        public ActionResult Details(int id)
        {
            var concert = session.Load<Concert>(id);

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
            var concerts = session.Query<Concert>()
                                  .Where(c => c.DateAndStartTime >= DateTime.Parse(year + "-08-01") && 
                                              c.DateAndStartTime <= DateTime.Parse(year + 1 + "-07-31"))
                                  .Where(c => c.IsPublished)
                                  .ToList();

            ViewBag.ConcertSeason = string.Format("{0}-{1}", year, year + 1);

            return View("Archive", concerts);
        }

        //
        // GET: /Concerts/Archived/

        public ActionResult Archived()
        {
            return View("ArchiveIndex");
        }

        //
        // GET: /Concerts/Create
        [Authorize]
        public ActionResult Create()
        {
            return View("Create");
        }

        //
        // POST: /Concerts/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(ConcertViewModel concertViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", concertViewModel);
            }

            session.Store(concertViewModel.ToModel<Concert, ConcertViewModel>());
            session.SaveChanges();

            return RedirectToAction("List");
        }

        //
        // GET: /Concerts/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var concert = session.Load<Concert>(id);

            if (concert == null)
            {
                return HttpNotFound();
            }

            return View("Edit", concert.ToViewModel<Concert, ConcertViewModel>());
        }

        //
        // POST: /Concerts/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, ConcertViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", viewModel);
            }

            session.Store(viewModel.ToModel<Concert, ConcertViewModel>());
            session.SaveChanges();

            return RedirectToAction("List");
        }

        //
        // GET: /Concerts/List

        [Authorize]
        public ActionResult List()
        {
            return View("List");
        }
    }
}

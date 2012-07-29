using System;
using System.Linq;
using System.Web.Mvc;
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

        private readonly Concert[] concerts = new[]
        {
            new Concert(1, "CGO Plays Music from Germany and Austria", new DateTime(2012, 04, 14, 20, 0, 0), "West Road Concert Hall"),
            new Concert(2, "CGO around the World", new DateTime(2012, 06, 29, 20, 0, 0), "West Road Concert Hall"),
            new Concert(3, "Russian Heritage", new DateTime(2012, 12, 01, 20, 0, 0), "West Road Concert Hall")
        };

        //
        // GET: /Concerts/

        public ActionResult Index()
        {
            return View("Index", concerts.OrderByDescending(c => c.DateAndStartTime));
        }

        //
        // GET: /Concerts/Details/5

        public ActionResult Details(int id)
        {
            var concert = session.Load<Concert>(id);
            return View("Details", concert);
        }

        //
        // GET: /Concerts/Create
        //[Authorize]
        public ActionResult Create()
        {
            return View("Create");
        }

        //
        // POST: /Concerts/Create

        [HttpPost]
        //[Authorize]
        public ActionResult Create(ConcertViewModel concert)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", concert);
                }

                return RedirectToAction("List");
            }
            catch
            {
                return View(concert);
            }
        }

        //
        // GET: /Concerts/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Concerts/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Concerts/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Concerts/Delete/5

        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult List()
        {
            throw new NotImplementedException();
        }
    }
}

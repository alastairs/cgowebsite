using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class RehearsalsController : Controller
    {
        private readonly Rehearsal[] rehearsals = new[]
                {
                    new Rehearsal(1, new DateTime(2012, 10, 7, 19, 30, 00), new DateTime(2012, 10, 7, 22, 00, 00), "Hughes Hall"),
                    new Rehearsal(2, new DateTime(2012, 10, 14, 19, 30, 00), new DateTime(2012, 10, 14, 22, 00, 00), "Hughes Hall"),
                    new Rehearsal(3, new DateTime(2012, 10, 21, 19, 30, 00), new DateTime(2012, 10, 21, 22, 00, 00), "Hughes Hall")
                };
        //
        // GET: /Rehearsals/

        public ActionResult Index()
        {
            return View("Index", rehearsals.Where(r => r.DateAndStartTime > DateTime.Now).OrderBy(r => r.DateAndStartTime));
        }

        //
        // GET: /Rehearsals/Details/5

        public ActionResult Details(int id)
        {
            return View("Details", rehearsals.Single(r => r.Id == id));
        }

        //
        // GET: /Rehearsals/Create

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Rehearsals/Create

        [HttpPost, Authorize]
        public ActionResult Create(Rehearsal collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Rehearsals/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Rehearsals/Edit/5

        [HttpPost, Authorize]
        public ActionResult Edit(int id, Rehearsal collection)
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
        // GET: /Rehearsals/Delete/5

        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Rehearsals/Delete/5

        [HttpPost, Authorize]
        public ActionResult Delete(int id, Rehearsal collection)
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
    }
}

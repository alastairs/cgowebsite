﻿using System;
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
            var concerts = new[]
            {
                        new Concert(1, "CGO Plays Music from Germany and Austria", new DateTime(2012, 04, 14, 20, 0, 0), "West Road Concert Hall"),
                        new Concert(2, "CGO around the World", new DateTime(2012, 06, 29, 20, 0, 0), "West Road Concert Hall"),
                        new Concert(3, "Russian Heritage", new DateTime(2012, 12, 01, 20, 0, 0), "West Road Concert Hall")
            };

            return View("Index", concerts.OrderByDescending(c => c.DateAndStartTime));
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
        public ActionResult Create(ConcertViewModel concertViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", concertViewModel);
                }

                var dateAndStartTime = new DateTime(concertViewModel.Date.Year, concertViewModel.Date.Month, concertViewModel.Date.Day,
                                                    concertViewModel.StartTime.Hour, concertViewModel.StartTime.Minute, 00);
                var concert = new Concert(concertViewModel.Id, concertViewModel.Title, dateAndStartTime, concertViewModel.Location);
                session.Store(concert);

                session.SaveChanges();

                return RedirectToAction("List");
            }
            catch
            {
                return View(concertViewModel);
            }
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
        public ActionResult Edit(int id, ConcertViewModel collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("List");
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
            var concerts = session.Query<Concert>();
            return View("List", concerts.ToList().Select(c => c.ToViewModel<Concert, ConcertViewModel>()));
        }
    }
}

﻿using System;
using System.Linq;
using System.Web.Mvc;
using CGO.Domain;
using CGO.Web.Mappers;
using CGO.Web.Models;
using CGO.Web.ViewModels;

using Raven.Client;

namespace CGO.Web.Controllers
{
    public class ConcertsController : Controller
    {
        private readonly IDocumentSession session;
        private readonly IConcertDetailsService concertDetailsService;
        private readonly IConcertsSeasonService concertsSeasonService;

        public ConcertsController(IDocumentSession session, IConcertDetailsService concertDetailsService, IConcertsSeasonService concertsSeasonService)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            if (concertDetailsService == null)
            {
                throw new ArgumentNullException("concertDetailsService");
            }

            if (concertsSeasonService == null)
            {
                throw new ArgumentNullException("concertsSeasonService");
            }

            this.session = session;
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

            concertDetailsService.SaveConcert(concertViewModel.ToModel<Concert, ConcertViewModel>());
            return RedirectToAction("List");
        }

        //
        // GET: /Concerts/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var concert = concertDetailsService.GetConcert(id);

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

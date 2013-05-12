﻿using System;
using System.Web.Mvc;
using CGO.Domain;
using CGO.Web.Mappers;
using CGO.Web.ViewModels;

namespace CGO.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ConcertsController : Controller
    {
        private readonly IConcertDetailsService concertDetailsService;

        public ConcertsController(IConcertDetailsService concertDetailsService)
        {
            if (concertDetailsService == null)
            {
                throw new ArgumentNullException("concertDetailsService");
            }

            this.concertDetailsService = concertDetailsService;
        }

        //
        // GET: /Admin/Concerts/

        public ActionResult Index()
        {
            return View("List");
        }

        //
        // GET: /Concerts/Create

        public ActionResult Create()
        {
            return View("Create");
        }

        //
        // POST: /Concerts/Create

        [HttpPost]
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
        public ActionResult Edit(int id, ConcertViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", viewModel);
            }

            concertDetailsService.SaveConcert(viewModel.ToModel<Concert, ConcertViewModel>());

            return RedirectToAction("List");
        }

        //
        // GET: /Concerts/List

        public ActionResult List()
        {
            return View("List");
        }
    }
}

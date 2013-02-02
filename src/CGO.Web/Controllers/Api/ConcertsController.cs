﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using CGO.Web.Mappers;
using CGO.Web.Models;
using CGO.Web.ViewModels;

using Raven.Client;

namespace CGO.Web.Controllers.Api
{
    public class ConcertsController : ApiController
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

        // GET api/concerts
        public IEnumerable<ConcertViewModel> Get()
        {
            var concerts = session.Query<Concert>();
            return concerts.ToList().Select(c => c.ToViewModel<Concert, ConcertViewModel>()).OrderByDescending(c => c.Date);
        }

        // GET api/concerts/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/concerts
        public HttpResponseMessage Post(ConcertViewModel concert)
        {
            if (concert == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            session.Store(concert.ToModel<Concert, ConcertViewModel>());
            session.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // PUT api/concerts/5
        public HttpResponseMessage Put(int id, ConcertViewModel concert)
        {
            if (concert == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        // DELETE api/concerts/5
        public HttpResponseMessage DeleteConcert(int id)
        {
            var concertToDelete = session.Load<Concert>(id);

            if (concertToDelete != null)
            {
                session.Delete(concertToDelete);
                session.SaveChanges();
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using CGO.Web.Models;

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
        public IEnumerable<Concert> Get()
        {
            var concerts = session.Query<Concert>();
            return concerts.ToList().OrderByDescending(c => c.DateAndStartTime);
        }

        // GET api/concerts/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/concerts
        public void Post(string value)
        {
        }

        // PUT api/concerts/5
        public void Put(int id, string value)
        {
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

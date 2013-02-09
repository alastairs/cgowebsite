using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CGO.Domain;
using CGO.Web.Mappers;
using CGO.Web.Models;
using CGO.Web.ViewModels.Api;

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
            return concerts.ToList().Select(c => c.ToViewModel<Concert, ConcertViewModel>()).OrderByDescending(c => c.DateAndStartTime);
        }

        // GET api/concerts/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/concerts
        public HttpResponseMessage Post(ConcertViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var concert = viewModel.ToModel<Concert, ConcertViewModel>();
            session.Store(concert);
            session.SaveChanges();
            viewModel = concert.ToViewModel<Concert, ConcertViewModel>();
            
            var response = Request.CreateResponse(HttpStatusCode.Created, viewModel);
            response.Headers.Location = new Uri(viewModel.Href, UriKind.Relative);
            return response;
        }

        // PUT api/concerts/5
        public HttpResponseMessage Put(int id, ConcertViewModel updatedConcert)
        {
            if (updatedConcert == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var originalConcert = session.Load<Concert>(id);
            if (originalConcert == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            originalConcert.ChangeTitle(updatedConcert.Title);
            originalConcert.ChangeDateAndStartTime(updatedConcert.DateAndStartTime);
            originalConcert.ChangeLocation(updatedConcert.Location);
            if (updatedConcert.IsPublished) originalConcert.Publish();
            session.SaveChanges();

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

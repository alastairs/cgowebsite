using System;
using System.Collections.Generic;
using System.Linq;

using CGO.Web.Infrastructure;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class ConcertsSideBar : SideBar
    {
        private readonly IDocumentSessionFactory documentSessionFactory;

        public ConcertsSideBar(IUrlHelper urlHelper, IDocumentSessionFactory documentSessionFactory) : base(urlHelper)
        {
            if (documentSessionFactory == null)
            {
                throw new ArgumentNullException("documentSessionFactory");
            }

            this.documentSessionFactory = documentSessionFactory;
        }

        public override IEnumerable<SideBarSection> GetSideBarSections()
        {
            var currentSeasonSection = new SideBarSection("Current Season");
            using (var session = documentSessionFactory.CreateSession())
            {
                var concerts = session.Query<Concert>().OrderBy(c => c.DateAndStartTime).ToList();

                foreach (var concert in concerts)
                {
                    currentSeasonSection.AddLink(new SideBarLink(concert.Title, Url.Action("Details", "Concerts", new { id = concert.Id }), false));
                }
            }

            return new[] { currentSeasonSection };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class ConcertsSideBar : SideBar
    {
        private readonly IDocumentSessionFactory documentSessionFactory;

        public ConcertsSideBar(UrlHelper urlHelper, IDocumentSessionFactory documentSessionFactory) : base(urlHelper)
        {
            if (documentSessionFactory == null)
            {
                throw new ArgumentNullException("documentSessionFactory");
            }

            this.documentSessionFactory = documentSessionFactory;
        }

        public override IEnumerable<SideBarSection> GetSideBarSections()
        {
            return new[]
                {
                    new SideBarSection("2012-13 Season", new[]
                        {
                            new SideBarLink("Russian Heritage 1 December 2012", Url.Action("Details", "Concerts", new {id = 3}), false),
                            new SideBarLink("The Witching Hour 8 March 2013", Url.Action("Details", "Concerts", new {id = 0}), false),
                            new SideBarLink("Beethoven 12 April 2013", Url.Action("Details", "Concerts", new {id = 0}), false),
                            new SideBarLink("28 June 2013", Url.Action("Details", "Concerts", new {id = 0}), false)
                        }),
                    new SideBarSection("Last Season", new[]
                        {
                            new SideBarLink("Music Inspired by Fairy Tales", Url.Action("Details", "Concerts", new {id = 0}), false),
                            new SideBarLink("The Planets", Url.Action("Details", "Concerts", new {id = 0}), false),
                            new SideBarLink("Music from Germany and Austria", Url.Action("Details", "Concerts", new {id = 1}), false),
                            new SideBarLink("CGO Around The World", Url.Action("Details", "Concerts", new {id = 2}), false)
                        }),
                    new SideBarSection("Older", new[]
                        {
                            new SideBarLink("2010-11 Season", Url.Action("Archive", "Concerts", new {year = 2010}), false),
                            new SideBarLink("2009-10 Season", Url.Action("Archive", "Concerts", new {year = 2009}), false),
                            new SideBarLink("2008-09 Season", Url.Action("Archive", "Concerts", new {year = 2008}), false),
                            new SideBarLink("2007-08 Season", Url.Action("Archive", "Concerts", new {year = 2007}), false),
                            new SideBarLink("2006-07 Season", Url.Action("Archive", "Concerts", new {year = 2006}), false),
                            new SideBarLink("2005-06 Season", Url.Action("Archive", "Concerts", new {year = 2005}), false)
                        })
                };
        }
    }
}
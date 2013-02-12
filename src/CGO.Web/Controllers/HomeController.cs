using System;
using System.Web.Mvc;
using CGO.Domain;
using CGO.Web.Infrastructure;
using CGO.Web.Models;

namespace CGO.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Cambridge Graduate Orchestra";
            ViewBag.GoogleLoginUrl = new GoogleOAuthConfiguration().MakeLoginUri("");

            var model =
                new HomePageViewModel(
                    new Concert(3, "Russian Heritage", new DateTime(2012, 12, 01, 20, 0, 0), "West Road Concert Hall"),
                    new[]
                        {
                            new Rehearsal(1, new DateTime(2012, 10, 7, 19, 30, 00), new DateTime(2012, 10, 7, 22, 00, 00), "Hughes Hall"),
                            new Rehearsal(2, new DateTime(2012, 10, 14, 19, 30, 00), new DateTime(2012, 10, 14, 22, 00, 00), "Hughes Hall"),
                            new Rehearsal(3, new DateTime(2012, 10, 21, 19, 30, 00), new DateTime(2012, 10, 21, 22, 00, 00), "Hughes Hall")
                        });

            Response.AppendHeader("X-XRDS-Location", new Uri(Request.Url, Response.ApplyAppPathModifier("~/Home/xrds")).AbsoluteUri);

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Xrds()
        {
            return View("Xrds");
        }
    }
}

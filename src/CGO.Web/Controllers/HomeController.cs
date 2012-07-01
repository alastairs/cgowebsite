using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGO.Web.Infrastructure;

namespace CGO.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Cambridge Graduate Orchestra";
            ViewBag.GoogleLoginUrl = new GoogleOAuthConfiguration().MakeLoginUri("");
            return View();
        }
    }
}

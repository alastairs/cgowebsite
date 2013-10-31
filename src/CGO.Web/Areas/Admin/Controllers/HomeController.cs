using System.Web.Mvc;

namespace CGO.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        //
        // GET: /Admin/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}

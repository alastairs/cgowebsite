using System.Configuration;
using System.Web.Mvc;

namespace CGO.Web.Areas.Admin.Controllers
{
    public class DiagnosticsController : Controller
    {
        //
        // GET: /Admin/Diagnostics/

        public ActionResult Index()
        {
            return View(ConfigurationManager.ConnectionStrings);
        }

    }
}

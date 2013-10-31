using System.Configuration;
using System.Web.Mvc;

namespace CGO.Web.Areas.Admin.Controllers
{
    public class DiagnosticsController : AdminController
    {
        //
        // GET: /Admin/Diagnostics/

        public ActionResult Index()
        {
            return View(ConfigurationManager.ConnectionStrings);
        }

    }
}

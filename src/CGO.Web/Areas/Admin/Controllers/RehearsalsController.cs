using System.Web.Mvc;
using CGO.Web.Areas.Admin.Models;

namespace CGO.Web.Areas.Admin.Controllers
{
    public class RehearsalsController : AdminController
    {
        //
        // GET: /Admin/Rehearsals/Create
        public ActionResult Create()
        {
            return View(new RehearsalViewModel());
        }

        //
        // POST: /Admin/Rehearsals/Create
        [HttpPost]
        public ActionResult Create(RehearsalViewModel viewModel)
        {
            return View();
        }
	}
}
using System.Web.Mvc;

namespace CGO.Web.Areas.Admin.Controllers
{
    [Authorize]
    public abstract class AdminController : Controller { }
}
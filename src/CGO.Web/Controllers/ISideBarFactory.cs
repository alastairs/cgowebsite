using CGO.Web.Infrastructure;

namespace CGO.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISideBarFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlHelper"> </param>
        /// <returns></returns>
        SideBar CreateSideBar(IUrlHelper urlHelper);
    }
}

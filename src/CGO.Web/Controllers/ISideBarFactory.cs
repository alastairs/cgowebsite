using System.Web.Mvc;

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
        SideBar CreateSideBar(UrlHelper urlHelper);
    }
}

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
        /// <param name="controllerName"> </param>
        /// <param name="documentSessionFactory"> </param>
        /// <returns></returns>
        SideBar CreateSideBar(IUrlHelper urlHelper, string controllerName, IDocumentSessionFactory documentSessionFactory);
    }
}

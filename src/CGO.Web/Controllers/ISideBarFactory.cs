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
        /// <param name="controllerName"> </param>
        /// <param name="documentSessionFactory"> </param>
        /// <returns></returns>
        SideBar CreateSideBar(UrlHelper urlHelper, string controllerName, IDocumentSessionFactory documentSessionFactory);
    }
}

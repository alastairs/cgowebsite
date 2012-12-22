using System;
using System.Web.Mvc;

namespace CGO.Web.Infrastructure
{
    public class MvcUrlHelper : IUrlHelper
    {
        private readonly UrlHelper urlHelper;

        public MvcUrlHelper(UrlHelper urlHelper)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException("urlHelper");
            }

            this.urlHelper = urlHelper;
        }

        public string Action(string actionName, string controllerName, object routeValues = null)
        {
            return urlHelper.Action(actionName, controllerName, routeValues);
        }
    }
}
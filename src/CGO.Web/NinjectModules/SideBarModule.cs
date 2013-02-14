using System.Web;
using System.Web.Mvc;
using CGO.Web.Controllers;
using Ninject.Modules;
using Ninject.Web.Common;

namespace CGO.Web.NinjectModules
{
    public class SideBarModule : NinjectModule
    {
        public override void Load()
        {
            const string concertsControllerName = "Concerts";

            Kernel.Bind<ISideBarFactory>()
                  .To<DefaultSideBarFactory>()
                  .InRequestScope();

            Kernel.Bind<ISideBarFactory>()
                  .To<ConcertsSideBarFactory>()
                  .When(_ => RequestedControllerIs(concertsControllerName))
                  .InRequestScope();
        }

        private static bool RequestedControllerIs(string controllerName)
        {
            const string controllerRouteKey = "controller";

            var mvcHandler = (MvcHandler)HttpContext.Current.Handler;
            var routeValues = mvcHandler.RequestContext.RouteData.Values;

            object requestedControllerName;
            if (routeValues.TryGetValue(controllerRouteKey, out requestedControllerName))
            {
                return requestedControllerName.ToString().ToUpper() == controllerName.ToUpper();
            }

            return false;
        }
    }
}
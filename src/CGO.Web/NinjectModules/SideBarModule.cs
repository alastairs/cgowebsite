using CGO.Web.Controllers;
using Ninject.Modules;
using Ninject.Web.Common;

namespace CGO.Web.NinjectModules
{
    public class SideBarModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ISideBarFactory>()
                  .To<ConcertsSideBarFactory>()
                  .WhenInjectedInto<ConcertsController>()
                  .InRequestScope();

            Kernel.Bind<ISideBarFactory>().To<DefaultSideBarFactory>().InRequestScope();
        }
    }
}
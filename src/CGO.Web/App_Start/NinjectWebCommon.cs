using CGO.Web.Controllers;
using Ninject.Extensions.Factory;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

[assembly: WebActivator.PreApplicationStartMethod(typeof(CGO.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(CGO.Web.App_Start.NinjectWebCommon), "Stop")]

namespace CGO.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Extensions.Conventions;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(a => a.FromAssembliesMatching("CGO.*.dll")
                              .SelectAllClasses()
                              .BindAllInterfaces()
                              .Configure(bind => bind.InRequestScope()));

            kernel.Bind<IDocumentStore>().ToMethod(_ =>
            {
                var documentStore = new EmbeddableDocumentStore
                {
                    DataDirectory = "CGO.raven"
                };

                documentStore.InitializeProfiling();
                documentStore.Initialize();
                
                return documentStore;
            }).InSingletonScope();
        }
    }
}

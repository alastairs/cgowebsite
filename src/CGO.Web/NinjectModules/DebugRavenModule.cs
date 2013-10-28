#if DEBUG
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

using Raven.Client;
using Raven.Client.Embedded;

namespace CGO.Web.NinjectModules
{
    public class DebugRavenModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDocumentStore>().ToMethod(_ => InitialiseDocumentStore()).InSingletonScope();
            Kernel.Bind<IDocumentSession>().ToMethod(_ => Kernel.Get<IDocumentStore>().OpenSession()).InRequestScope();
        }

        private static IDocumentStore InitialiseDocumentStore()
        {
            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = "CGO.raven",
                Configuration = { Port = 28645 }
            };

            documentStore.Initialize();
            documentStore.InitializeProfiling();

            return documentStore;
        }
    }
}
#endif
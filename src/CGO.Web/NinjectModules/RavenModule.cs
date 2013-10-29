using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace CGO.Web.NinjectModules
{
    public class RavenModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDocumentStore>().ToMethod(_ => InitialiseDocumentStore()).InSingletonScope();
            Kernel.Bind<IDocumentSession>().ToMethod(_ => Kernel.Get<IDocumentStore>().OpenSession()).InRequestScope();
        }

        private static IDocumentStore InitialiseDocumentStore()
        {
#if DEBUG
            var documentStore = new EmbeddableDocumentStore();
#else
            var documentStore = new DocumentStore();
#endif

            documentStore.ConnectionStringName = "RavenHQ";
            documentStore.Initialize();

            return documentStore;
        }
    }
}

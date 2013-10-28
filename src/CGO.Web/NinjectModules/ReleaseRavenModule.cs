#if !DEBUG
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

using Raven.Client;
using Raven.Client.Document;

namespace CGO.Web.NinjectModules
{
    public class ReleaseRavenModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDocumentStore>().ToMethod(_ => InitialiseDocumentStore()).InSingletonScope();
            Kernel.Bind<IDocumentSession>().ToMethod(_ => Kernel.Get<IDocumentStore>().OpenSession()).InRequestScope();
        }

        private static IDocumentStore InitialiseDocumentStore()
        {
            var documentStore = new DocumentStore
            {
                ConnectionStringName = "RavenHQ"
            };

            documentStore.Initialize();

            return documentStore;
        }
    }
}
#endif
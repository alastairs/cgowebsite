using NUnit.Framework;

using Raven.Client;
using Raven.Client.Embedded;

namespace CGO.Web.Tests.Controllers
{
    public abstract class RavenTest
    {
        protected IDocumentStore Store { get; private set; }
        protected IDocumentSession Session { get; private set; }

        [SetUp]
        public void ConfigureRavenDb()
        {
            Store = new EmbeddableDocumentStore { RunInMemory = true };
            Store.Initialize();
        }

        [SetUp]
        public void OpenSession()
        {
            Session = Store.OpenSession();
        }

        [TearDown]
        public void CloseSession()
        {
            Session.Dispose();
        }

        [TearDown]
        public void DestroyRavenDbStore()
        {
            Store.Dispose();
        }
    }
}
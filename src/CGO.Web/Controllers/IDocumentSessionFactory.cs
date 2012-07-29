using System;
using Raven.Client;

namespace CGO.Web.Controllers
{
    public interface IDocumentSessionFactory
    {
        IDocumentSession CreateSession();
    }

    public class DocumentSessionFactory : IDocumentSessionFactory
    {
        private readonly IDocumentStore documentStore;

        public DocumentSessionFactory(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException("documentStore");
            }

            this.documentStore = documentStore;
        }

        public IDocumentSession CreateSession()
        {
            return documentStore.OpenSession();
        }
    }
}
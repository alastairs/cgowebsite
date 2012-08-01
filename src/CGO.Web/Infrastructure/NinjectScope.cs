using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Syntax;

namespace CGO.Web.Infrastructure
{
    public class NinjectScope : IDependencyScope
    {
        protected IResolutionRoot ResolutionRoot { get; private set; }

        public NinjectScope(IResolutionRoot kernel)
        {
            ResolutionRoot = kernel;
        }

        public object GetService(Type serviceType)
        {
            IRequest request = ResolutionRoot.CreateRequest(serviceType, null, Enumerable.Empty<Parameter>(), true, true);
            return ResolutionRoot.Resolve(request).SingleOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IRequest request = ResolutionRoot.CreateRequest(serviceType, null, Enumerable.Empty<Parameter>(), true, true);
            return ResolutionRoot.Resolve(request);
        }

        public void Dispose()
        {
            var disposable = (IDisposable)ResolutionRoot;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            ResolutionRoot = null;
        }
    }
}
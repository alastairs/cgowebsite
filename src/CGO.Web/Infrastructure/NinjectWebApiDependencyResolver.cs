using System;
using System.Web.Http.Dependencies;

using Ninject;

namespace CGO.Web.Infrastructure
{
    public class NinjectWebApiDependencyResolver : NinjectScope, IDependencyResolver
    {
        private readonly IKernel kernel;

        public NinjectWebApiDependencyResolver(IKernel kernel) : base(kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            this.kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectScope(kernel.BeginBlock());
        }
    }
}
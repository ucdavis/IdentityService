using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Syntax;

namespace IdentityService
{
    public class NinjectWebApiScope  : IDependencyScope
    {
           protected IResolutionRoot resolutionRoot;

    public NinjectWebApiScope(IResolutionRoot resolutionRoot)
    {
        this.resolutionRoot = resolutionRoot;
    }

    public object GetService(Type serviceType)
    {
        return resolutionRoot.Resolve(this.CreateRequest(serviceType)).SingleOrDefault();
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return resolutionRoot.Resolve(this.CreateRequest(serviceType)).ToList();
    }

    private IRequest CreateRequest(Type serviceType)
    {
        return resolutionRoot.CreateRequest(serviceType,
                                null,
                                new Parameter[0],
                                true,
                                true);
    }

    public void Dispose()
    {
        resolutionRoot = null;
    }
}
    }

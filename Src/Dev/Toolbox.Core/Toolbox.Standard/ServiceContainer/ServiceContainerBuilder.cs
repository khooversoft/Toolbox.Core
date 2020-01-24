using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ServiceContainerBuilder
    {
        public ServiceContainerBuilder() { }

        public Func<Type, object>? GetService { get; set; }

        public Func<Type, object>? GetServiceOptional { get; set; }

        public Func<string, IEnumerable<Type>, IDisposable>? CreateScope { get; set; }

        public ServiceContainerBuilder SetService(Func<Type, object> getService)
        {
            GetService = getService;
            return this;
        }

        public ServiceContainerBuilder SetServiceOptional(Func<Type, object>? getServiceOptional)
        {
            GetServiceOptional = getServiceOptional;
            return this;
        }

        public ServiceContainerBuilder SetCreateScope(Func<string, IEnumerable<Type>, IDisposable> createScop)
        {
            CreateScope = createScop;
            return this;
        }

        public IServiceContainer Build()
        {
            return new ServiceContainer(GetService, GetServiceOptional, CreateScope);
        }
    }
}

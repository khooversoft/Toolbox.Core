using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class ServiceProviderExtensions
    {
        public static T Resolve<T>(this IServiceProvider service)
        {
            return (T)service.GetService(typeof(T));
        }
    }
}

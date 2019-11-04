using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public interface IPipeline<TContext, T>
    {
        bool Post(TContext context, T message);
    }
}

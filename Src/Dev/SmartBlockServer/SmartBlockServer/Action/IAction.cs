using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartBlockServer
{
    internal interface IAction
    {
        Task Run(IWorkContext context);
    }
}

using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Run
{
    [DebuggerDisplay("Name={Name}")]
    public class Activity : IActivity
    {
        public Activity(string name, Func<IWorkContext, IRunContext, Task> funcAsync)
        {
            name.VerifyNotEmpty(nameof(name));
            funcAsync.VerifyNotNull(nameof(funcAsync));

            Name = name;
            FuncAsync = funcAsync;
        }

        public Activity(string name, Action<IWorkContext, IRunContext> func)
        {
            name.VerifyNotEmpty(nameof(name));
            func.VerifyNotNull(nameof(func));

            Name = name;
            Func = func;
        }

        public string Key => Name;

        public string Name { get; }

        public Func<IWorkContext, IRunContext, Task>? FuncAsync { get; }

        public Action<IWorkContext, IRunContext>? Func { get; }

        public IProperty Properties { get; } = new Property();

        public async Task Run(IWorkContext context, IRunContext runContext)
        {
            try
            {
                if (FuncAsync != null)
                {
                    await FuncAsync(context, new RunContext(runContext, this));
                }
                else
                {
                    Func!(context, new RunContext(runContext, this));
                }
            }
            catch (Exception ex)
            {
                Properties.SetFailed("Run failed", ex);
            }
        }

        public IActivity WithName(string name) => FuncAsync != null ? new Activity(name, FuncAsync) : new Activity(name, Func!);
    }
}

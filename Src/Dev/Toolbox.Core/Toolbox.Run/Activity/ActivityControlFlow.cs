using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Run
{
    [DebuggerDisplay("FromActivityName={FromActivityName}, ToActivityName={ToActivityName}")]
    public class ActivityControlFlow : GraphEdge<string>, IControlFlow
    {
        public ActivityControlFlow(string fromActivityName, string toActivityName, Func<IWorkContext, IRunContext, Task<bool>> predicateAsync)
            : base(fromActivityName, toActivityName)
        {
            fromActivityName.VerifyNotEmpty(nameof(fromActivityName));
            toActivityName.VerifyNotEmpty(nameof(toActivityName));
            predicateAsync.VerifyNotNull(nameof(predicateAsync));

            PredicateAsync = predicateAsync;
        }
        
        public ActivityControlFlow(string fromActivityName, string toActivityName, Func<IWorkContext, IRunContext, bool> predicate)
            : base(fromActivityName, toActivityName)
        {
            fromActivityName.VerifyNotEmpty(nameof(fromActivityName));
            toActivityName.VerifyNotEmpty(nameof(toActivityName));
            predicate.VerifyNotNull(nameof(predicate));

            Predicate = predicate;
        }

        public string FromActivityName => FromNodeKey;

        public string ToActivityName => ToNodeKey;

        public Func<IWorkContext, IRunContext, Task<bool>>? PredicateAsync { get; }

        public Func<IWorkContext, IRunContext, bool>? Predicate { get; }

        public async Task<bool> IsValid(IWorkContext context, IRunContext runContext) => PredicateAsync != null 
            ? await PredicateAsync(context, runContext) 
            : Predicate!(context, runContext);

        public ActivityControlFlow WithName(string fromName, string toName) => PredicateAsync != null
            ? new ActivityControlFlow(fromName, toName, PredicateAsync)
            : new ActivityControlFlow(fromName, toName, Predicate!);
    }
}

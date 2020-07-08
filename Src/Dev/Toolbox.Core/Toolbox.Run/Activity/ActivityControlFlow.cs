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
        public ActivityControlFlow(string fromActivityName, string toActivityName, Func<IRunContext, IActivity, Task<bool>> predicateAsync)
            : base(fromActivityName, toActivityName)
        {
            fromActivityName.VerifyNotEmpty(nameof(fromActivityName));
            toActivityName.VerifyNotEmpty(nameof(toActivityName));
            predicateAsync.VerifyNotNull(nameof(predicateAsync));

            PredicateAsync = predicateAsync;
        }
        
        public ActivityControlFlow(string fromActivityName, string toActivityName, Func<IRunContext, IActivity, bool> predicate)
            : base(fromActivityName, toActivityName)
        {
            fromActivityName.VerifyNotEmpty(nameof(fromActivityName));
            toActivityName.VerifyNotEmpty(nameof(toActivityName));
            predicate.VerifyNotNull(nameof(predicate));

            Predicate = predicate;
        }

        public string FromActivityName => FromNodeKey;

        public string ToActivityName => ToNodeKey;

        public Func<IRunContext, IActivity, Task<bool>>? PredicateAsync { get; }

        public Func<IRunContext, IActivity, bool>? Predicate { get; }

        public async Task<bool> IsValid(IRunContext runContext, IActivity activity) => PredicateAsync != null 
            ? await PredicateAsync(runContext, activity) 
            : Predicate!(runContext, activity);

        public ActivityControlFlow WithName(string fromName, string toName) => PredicateAsync != null
            ? new ActivityControlFlow(fromName, toName, PredicateAsync)
            : new ActivityControlFlow(fromName, toName, Predicate!);
    }
}

using KHooversoft.Toolbox.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Run
{
    public class ControlFlow : IActivityCommon
    {
        public ControlFlow(Func<IWorkContext, IRunContext, Task<bool>> predicate)
        {
            predicate.VerifyNotNull(nameof(predicate));

            PredicateAsync = predicate;
        }

        public ControlFlow(Func<IWorkContext, IRunContext, bool> predicate)
        {
            predicate.VerifyNotNull(nameof(predicate));

            Predicate = predicate;
        }

        public ControlFlow(Func<IWorkContext, IRunContext, Task<bool>> predicate, IActivity toActivity)
            : this(predicate)
        {
            toActivity.VerifyNotNull(nameof(toActivity));

            ToActivity = toActivity;
        }

        public ControlFlow(Func<IWorkContext, IRunContext, bool> predicate, IActivity toActivity)
            : this(predicate)
        {
            toActivity.VerifyNotNull(nameof(toActivity));

            ToActivity = toActivity;
        }

        public IActivity? ToActivity { get; private set; }

        public Func<IWorkContext, IRunContext, Task<bool>>? PredicateAsync { get; }

        public Func<IWorkContext, IRunContext, bool>? Predicate { get; }

        public static ControlFlow operator +(ControlFlow subject, IActivity activity)
        {
            subject.ToActivity = activity;
            return subject;
        }
    }
}

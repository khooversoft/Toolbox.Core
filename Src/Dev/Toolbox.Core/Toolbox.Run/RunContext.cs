using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Khooversoft.Toolbox.Run
{
    public class RunContext : IRunContext
    {
        public RunContext() => Property = new Property();

        public RunContext(IRunContext runContext, IActivity activity)
        {
            runContext.VerifyNotNull(nameof(runContext));
            activity.VerifyNotNull(nameof(activity));

            Property = runContext.Property;
            Activity = activity;
        }

        public IProperty Property { get; }

        public IActivity? Activity { get; }

        public IRunContext With(IActivity activity)
        {
            activity.VerifyNotNull(nameof(activity));

            return new RunContext(this, activity);
        }
    }

    //public interface IRunContext<T> : IRunContext
    //{
    //    T Message { get; }
    //}

    //public class RunContext<T> : IRunContext<T>
    //{
    //    public RunContext(T message)
    //    {
    //        message.VerifyNotNull(nameof(message));
    //        Message = message;
    //        Property = new Property();
    //    }

    //    public RunContext(IRunContext<T> runContext, IActivity activity)
    //    {
    //        runContext.VerifyNotNull(nameof(runContext));
    //        activity.VerifyNotNull(nameof(activity));

    //        Message = runContext.Message;
    //        Property = runContext.Property;
    //        Activity = activity;
    //    }

    //    public T Message { get; }

    //    public IProperty Property { get; }

    //    public IActivity? Activity { get; }

    //    public IRunContext With(IActivity activity)
    //    {
    //        activity.VerifyNotNull(nameof(activity));

    //        return new RunContext(this, activity);
    //    }
    //}
}
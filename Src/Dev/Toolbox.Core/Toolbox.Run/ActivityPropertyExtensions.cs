using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Run
{
    public static class ActivityPropertyExtensions
    {
        public const string ExitState = "ExitState";
        public const string ExitMessage = "ExitMessage";
        public const string Exception = "Exception";
        public const string Message = "Message";

        public static void SetSuccess(this IProperty property) => property
            .VerifyNotNull(nameof(property))
            .Set(ExitState, true);

        public static bool IsSuccess(this IProperty property) => property
            .VerifyNotNull(nameof(property))
            .TryGetValue(ExitState, out bool value) && value == true;

        public static void SetFailed(this IProperty property, string message, Exception? exception = null)
        {
            property.VerifyNotNull(nameof(property));
            message.VerifyNotNull(nameof(message));

            property.Set(ExitState, false);
            property.Set(ExitMessage, message);

            if (exception != null) property.Set(Exception, exception.ToString());
        }

        public static void SetMessage<T>(this IProperty property, T message)
        {
            property.VerifyNotNull(nameof(property));
            message.VerifyNotNull(nameof(message));

            property.Set(Message, message);
        }

        public static T GetMessage<T>(this IProperty property) => property
            .VerifyNotNull(nameof(property))
            .Get<T>(Message);

        public static IRunContext SetMessage<T>(this IRunContext runContext, T message)
        {
            runContext.VerifyNotNull(nameof(runContext));
            message.VerifyNotNull(nameof(message));

            runContext.Property.Set(Message, message);
            return runContext;
        }

        public static T GetMessage<T>(this IRunContext runContext) => runContext
            .VerifyNotNull(nameof(runContext))
            .Property.Get<T>(Message);
    }
}

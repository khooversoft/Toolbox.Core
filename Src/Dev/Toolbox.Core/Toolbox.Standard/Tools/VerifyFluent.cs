// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class VerifyFluent
    {
        [DebuggerStepThrough]
        public static VerifyContext<T> Verify<T>(this T subject)
        {
            return new VerifyContext<T>(subject);
        }

        [DebuggerStepThrough]
        public static VerifyContext<T> Verify<T>(this T subject, string name)
        {
            return new VerifyContext<T>(subject, name);
        }

        [DebuggerStepThrough]
        public static VerifyContext<bool> Assert(this VerifyContext<bool> context, string message)
        {
            if (!context.Value) throw new ArgumentException(context.Format(message));
            return context;
        }

        [DebuggerStepThrough]
        public static VerifyContext<T> Assert<T>(this VerifyContext<T> context, Func<T, bool> assertOpr, string message)
        {
            if (!assertOpr(context.Value)) throw new ArgumentException(context.Format(message));
            return context;
        }

        [DebuggerStepThrough]
        public static VerifyContext<T> Assert<T>(this VerifyContext<T> context, Func<T, bool> assertOpr, Func<T, string> message)
        {
            if (!assertOpr(context.Value)) throw new ArgumentException(context.Format(message(context.Value)));
            return context;
        }

        [DebuggerStepThrough]
        public static VerifyContext<T> Assert<T, TException>(this VerifyContext<T> context, Func<T, bool> assertOpr, string message)
        {
            if (!assertOpr(context.Value)) throw (Exception)Activator.CreateInstance(typeof(TException), context.Format(message));
            return context;
        }

        [DebuggerStepThrough]
        public static VerifyContext<string> IsNotEmpty(this VerifyContext<string> context, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(context.Value)) throw new ArgumentNullException(context.Format(message));
            return context;
        }

        [DebuggerStepThrough]
        public static VerifyContext<T> IsNotNull<T>(this VerifyContext<T> context, string? message = null)
        {
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
            if (EqualityComparer<T>.Default.Equals(context.Value, default)) throw new ArgumentNullException(context.Format(message));
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.

            return context;
        }
    }
}

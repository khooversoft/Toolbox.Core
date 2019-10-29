// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Toolbox.Standard
{
    public struct VerifyContext<T>
    {
        public VerifyContext(string name, T value)
        {
            Verify.Assert<ArgumentNullException>(!name.IsEmpty(), nameof(name));

            Name = name;
            Value = value;
        }

        public string Name { get; }

        public T Value { get; }
    }

    public static class VerifyFluent
    {
        public static VerifyContext<T> Verify<T>(this T subject, string name)
        {
            return new VerifyContext<T>(name, subject);
        }

        public static VerifyContext<T> Assert<T>(this VerifyContext<T> context, Func<T, bool> assertOpr, string message)
        {
            if (!assertOpr(context.Value)) throw new ArgumentException(message);
            return context;
        }

        public static VerifyContext<T> Assert<T, TException>(this VerifyContext<T> context, Func<T, bool> assertOpr, string message)
        {
            if (!assertOpr(context.Value)) throw (Exception)Activator.CreateInstance(typeof(T), message);
            return context;
        }

        public static VerifyContext<string> IsNotEmpty(this VerifyContext<string> context)
        {
            if (string.IsNullOrWhiteSpace(context.Value)) throw new ArgumentNullException(context.Name, "Null or empty");
            return context;
        }

        public static VerifyContext<T> IsNotNull<T>(this VerifyContext<T> context)
        {
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
            if (EqualityComparer<T>.Default.Equals(context.Value, default)) throw new ArgumentNullException(context.Name, "Null or empty");
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.

            return context;
        }
    }
}

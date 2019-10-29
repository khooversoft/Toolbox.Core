// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Toolbox.Standard
{
    public static class Verify
    {
        /// <summary>
        /// Assert test
        /// </summary>
        /// <param name="state">state to test</param>
        /// <param name="message">message</param>
        [DebuggerStepThrough]
        public static void Assert(bool state, string message)
        {
            if (!state) throw new ArgumentException(message);
        }

        /// <summary>
        /// Assert test and throw exception with message
        /// </summary>
        /// <typeparam name="T">type of exception</typeparam>
        /// <param name="test">test</param>
        /// <param name="message">exception message optional</param>
        [DebuggerStepThrough]
        public static void Assert<T>(bool test, string message) where T : Exception
        {
            if (test) return;
            message = message ?? throw new ArgumentException(nameof(message));

            throw (Exception)Activator.CreateInstance(typeof(T), message);
        }

        /// <summary>
        /// Insures string is not empty (null or empty)
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <param name="value">string to test</param>
        /// <param name="message">overrides default message (optional)</param>
        [DebuggerStepThrough]
        public static void IsNotEmpty(string name, string value, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Null or empty");
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(name, "Null or empty");
        }

        /// <summary>
        /// Insures string is not empty (null or empty)
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <param name="value">string to test</param>
        /// <param name="message">overrides default message (optional)</param>
        [DebuggerStepThrough]
        public static void IsNotNull<T>(string name, T value, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Null or empty");

#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
            if (EqualityComparer<T>.Default.Equals(value, default)) throw new ArgumentNullException(message, "Null or empty");
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
        }
    }
}

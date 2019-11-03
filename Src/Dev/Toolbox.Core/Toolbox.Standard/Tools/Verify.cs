// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Khooversoft.Toolbox.Standard
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
    }
}

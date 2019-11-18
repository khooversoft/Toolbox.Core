// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Khooversoft.Toolbox.Standard
{
    public struct VerifyContext<T>
    {
        public VerifyContext(T value)
        {
            Value = value;
            Name = null;
        }

        public VerifyContext(T value, string name)
        {
            Value = value;
            Name = name;
        }

        public T Value { get; }

        public string? Name { get; }

        public string Format(string? message)
        {
            return (Name == null ? string.Empty : Name + ": ") + (message ?? "Verification failed");
        }
    }
}

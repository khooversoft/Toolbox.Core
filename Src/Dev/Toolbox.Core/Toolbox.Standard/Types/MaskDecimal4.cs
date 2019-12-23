// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public struct MaskDecimal4
    {
        private const int mask = 10000;

        public MaskDecimal4(long value)
        {
            Value = value;
        }

        public MaskDecimal4(float value)
        {
            Value = (long)(value * mask);
        }

        public MaskDecimal4(double value)
        {
            Value = (long)(value * mask);
        }

        public long Value { get; }

        public double ToDouble() => (double)Value / mask;

        public static implicit operator double(MaskDecimal4 subject)
        {
            return subject.ToDouble();
        }

        public static implicit operator long(MaskDecimal4 subject)
        {
            return subject.Value;
        }

        public static implicit operator MaskDecimal4(long value)
        {
            return new MaskDecimal4(value);
        }

        public static implicit operator MaskDecimal4(float value)
        {
            return new MaskDecimal4(value);
        }

        public static explicit operator MaskDecimal4(double value)
        {
            return new MaskDecimal4(value);
        }
    }
}

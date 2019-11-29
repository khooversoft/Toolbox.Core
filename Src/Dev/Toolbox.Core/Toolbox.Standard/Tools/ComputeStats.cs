// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Stats represents a list of samples (floating point values)
    /// This class can calculate the standard statistics on this list
    /// (Mean, Median, StandardDeviation ...)
    /// </summary>
    public class ComputeStats : IEnumerable<float>
    {
        private readonly List<float> _data;
        private float _minimum;
        private float _maximum;
        private float _median;
        private float _mean;
        private float _standardDeviation;
        private bool _statsComputed;
        private object _lock = new object();

        public ComputeStats(IEnumerable<float>? values = null)
        {
            _data = values?.ToList() ?? new List<float>();
        }

        public int Count => _data.Count;

        public float this[int idx] => _data[idx];

        public IEnumerator<float> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

        public float Minimum
        {
            get
            {
                Compute();
                return _minimum;
            }
        }

        public float Maximum
        {
            get
            {
                Compute();
                return _maximum;
            }
        }

        public float Median
        {
            get
            {
                Compute();
                return _median;
            }
        }

        public float Mean
        {
            get
            {
                Compute();
                return _mean;
            }
        }

        public float StandardDeviation
        {
            get
            {
                Compute();
                return _standardDeviation;
            }
        }

        public void Add(params float[] dataItem)
        {
            _statsComputed = false;

            lock (_lock)
            {
                _data.AddRange(dataItem);
            }
        }

        public void Add(IEnumerable<float> dataItems)
        {
            dataItems.Verify(nameof(dataItems)).IsNotNull();
            _statsComputed = false;

            lock (_lock)
            {
                _data.AddRange(dataItems.ToArray());
            }
        }

        public override string ToString()
        {
            Compute();

            return "Count=" + _data.Count + " mean=" + _mean.ToString("f3") + " median=" + _median.ToString("f3") +
                   " min=" + _minimum.ToString("f3") + " max=" + _maximum.ToString("f3") +
                   " sdtdev=" + _standardDeviation.ToString("f3") + " samples=" + Count.ToString();
        }

        public void Compute()
        {
            if (_statsComputed) return;

            lock (_lock)
            {
                _minimum = float.MaxValue;
                _maximum = float.MinValue;
                _median = 0.0F;
                _mean = 0.0F;
                _standardDeviation = 0.0F;

                double total = 0;

                foreach (float dataPoint in this)
                {
                    if (dataPoint < _minimum) _minimum = dataPoint;
                    if (dataPoint > _maximum) _maximum = dataPoint;

                    total += dataPoint;
                }

                if (Count > 0)
                {
                    _data.Sort();

                    if (Count % 2 == 1)
                    {
                        _median = this[Count / 2];
                    }
                    else
                    {
                        _median = (this[(Count / 2) - 1] + this[Count / 2]) / 2;
                    }

                    _mean = (float)(total / Count);

                    double squares = 0.0;
                    foreach (float dataPoint in this)
                    {
                        double diffFromMean = dataPoint - _mean;
                        squares += diffFromMean * diffFromMean;
                    }

                    _standardDeviation = (float)Math.Sqrt(squares / Count);
                }

                _statsComputed = true;
            }
        }
    }
}

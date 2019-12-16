// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class MetricSamplerTests
    {
        [Fact]
        public void GivenSampler_WhenTestingFast_ShouldHaveSingleSummaryRecord()
        {
            const int count = 99;
            var metrics = new MetricSampler(TimeSpan.FromSeconds(1));

            metrics.Start();

            Enumerable.Range(0, count)
                .ForEach(x => metrics.Add(x));

            metrics.Stop();

            IReadOnlyList<MetricSample> metricsList = metrics.GetMetrics();
            metricsList.Count().Should().Be(1);

            int n = count - 1;
            float sumOfNumbers = (n * (n + 1)) / 2;
            metricsList.Max(x => x.Value).Should().Be(sumOfNumbers);
            metricsList.Max(x => x.Count).Should().Be(count);
        }
    }
}

// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using Xunit;

namespace MessageNet.Node.Tests
{
    public class NodeToNodeTests
    {
        private readonly TestServerFixture _fixture;
        private readonly IWorkContext _workContext = WorkContextBuilder.Default;

        public NodeToNodeTests(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Test1()
        {

        }
    }
}

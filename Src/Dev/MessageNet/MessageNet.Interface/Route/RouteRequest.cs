// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    /// <summary>
    /// Node registration request
    /// </summary>
    public class RouteRequest
    {
        public string? NetworkId { get; set; }

        public string? NodeId { get; set; }

        /// <summary>
        /// Test network id
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <returns>registration</returns>
        public static RouteRequest Test(string nodeId) => new RouteRequest { NetworkId = "Test", NodeId = nodeId.Verify().IsNotEmpty().Value };

        /// <summary>
        /// Production network id
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <returns>registration</returns>
        public static RouteRequest Production(string nodeId) => new RouteRequest { NetworkId = "Production", NodeId = nodeId.Verify().IsNotEmpty().Value };

        /// <summary>
        /// Staging network id
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <returns>registration</returns>
        public static RouteRequest Staging(string nodeId) => new RouteRequest { NetworkId = "Staging", NodeId = nodeId.Verify().IsNotEmpty().Value };
    }
}

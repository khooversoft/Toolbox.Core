// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Management;
using Khooversoft.Toolbox.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageHub.NameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private readonly IWorkContext _workContext;
        private readonly IRouteManager _routeManager;

        public AdministrationController(IWorkContext workContext, IRouteManager routeManager)
        {
            _workContext = workContext;
            _routeManager = routeManager;
        }

        [HttpPost("clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearAll()
        {
            await _routeManager.Clear(_workContext);
            return StatusCode(StatusCodes.Status200OK, new { Ok = true });
        }
    }
}
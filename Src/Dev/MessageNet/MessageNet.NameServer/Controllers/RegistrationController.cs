// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.MessageNet.Management;
using Khooversoft.Toolbox.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessageHub.NameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IWorkContext _workContext;

        public RegistrationController(IWorkContext workContext, IRouteRepository routeManager)
        {
            _workContext = workContext;
            _routeRepository = routeManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] RouteRequest routeRequest)
        {
            if (routeRequest == null) return StatusCode(StatusCodes.Status400BadRequest);

            NodeRegistration response = await _routeRepository.Register(_workContext, routeRequest);

            return StatusCode(StatusCodes.Status201Created, response.ConvertTo());
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Unregister([FromBody] RouteRequest routeRequest)
        {
            if (routeRequest == null) return StatusCode(StatusCodes.Status400BadRequest);

            await _routeRepository.Unregister(_workContext, routeRequest);

            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Lookup([FromBody] RouteRequest routeRequest)
        {
            if (routeRequest == null || routeRequest.NodeId.IsEmpty()) return StatusCode(StatusCodes.Status400BadRequest);

            IReadOnlyList<NodeRegistration> response = await _routeRepository.Search(_workContext, routeRequest);
            if (response == null || response.Count != 1) return StatusCode(StatusCodes.Status404NotFound);

            return Ok(response[0].ConvertTo());
        }
    }
}

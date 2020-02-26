// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            QueueId response = await _routeRepository.Register(_workContext, routeRequest);

            var routeResponse = new RouteResponse
            {
                Namespace = response.Namespace,
                NetworkId = response.NetworkId,
                NodeId = response.NodeId,
            };

            return StatusCode(StatusCodes.Status201Created, routeResponse);
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

        [HttpGet("{search}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Lookup(string search)
        {
            IReadOnlyList<QueueId> responses = await _routeRepository.Search(_workContext, search);

            var list = responses
                .Select(x => new RouteResponse
                {
                    Namespace = x.Namespace,
                    NetworkId = x.NetworkId,
                    NodeId = x.NodeId,
                })
                .ToList();

            return Ok(list);
        }
    }
}

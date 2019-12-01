using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khooversoft.MessageHub.Interface;
using Khooversoft.MessageHub.Management;
using Khooversoft.Toolbox.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessageHub.NameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistgrationController : ControllerBase
    {
        private readonly IRouteManager _routeManager;
        private readonly ILogger<RegistgrationController> _logger;
        private readonly IWorkContext _workContext;

        public RegistgrationController(IRouteManager routeManager, ILogger<RegistgrationController> logger, IWorkContext workContext)
        {
            _routeManager = routeManager;
            _logger = logger;
            _workContext = workContext;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Set([FromBody] RouteRegistrationRequest routeRegistrationRequest)
        {
            if (routeRegistrationRequest == null) return StatusCode(StatusCodes.Status400BadRequest);

            RouteRegistrationResponse response = await _routeManager.Register(_workContext, routeRegistrationRequest);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Lookup([FromBody] RouteLookupRequest routeLookupRequest)
        {
            if (routeLookupRequest == null || routeLookupRequest.SearchNodeId.IsEmpty()) return StatusCode(StatusCodes.Status400BadRequest);

            IReadOnlyList<RouteLookupResponse> response = await _routeManager.Search(_workContext, routeLookupRequest);
            if (response == null || response.Count != 1) return StatusCode(StatusCodes.Status404NotFound);

            return Ok(response[0]);
        }
    }
}

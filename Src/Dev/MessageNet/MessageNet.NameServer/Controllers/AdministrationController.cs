// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MessageHub.NameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private readonly IWorkContext _workContext;
        private readonly IRouteRepository _routeRepository;

        public AdministrationController(IWorkContext workContext, IRouteRepository routeRepository)
        {
            _workContext = workContext;
            _routeRepository = routeRepository;
        }

        [HttpPost("clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearAll()
        {
            await _routeRepository.Clear(_workContext);
            return StatusCode(StatusCodes.Status200OK, new { Ok = true });
        }
    }
}
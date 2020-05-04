using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PowerPlants.Domain.Models;
using PowerPlants.Domain.Services.Interfaces;

namespace PowerPlants.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PowerLoadController : ControllerBase
    {
        private readonly ILogger<PowerLoadController> _logger;
        private readonly IPowerLoadService _powerloadService;

        public PowerLoadController(ILogger<PowerLoadController> logger, IPowerLoadService powerloadService)
        {
            _logger = logger;
            _powerloadService = powerloadService;
        }

        [HttpPost("getpowerplants")]
        public ActionResult GetPowerPlants([FromBody] PowerRequest powerRequest)
        {
            try
            {
                var plants = _powerloadService.GetPowerPlants(powerRequest);

                return Ok(plants.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

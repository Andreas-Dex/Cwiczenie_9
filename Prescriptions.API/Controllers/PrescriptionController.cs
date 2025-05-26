using Microsoft.AspNetCore.Mvc;
using Prescriptions.API.DTOs;
using Prescriptions.API.Services;

namespace Prescriptions.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _svc;
        public PrescriptionController(IPrescriptionService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddPrescriptionRequest req)
        {
            try
            {
                await _svc.AddPrescriptionAsync(req);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
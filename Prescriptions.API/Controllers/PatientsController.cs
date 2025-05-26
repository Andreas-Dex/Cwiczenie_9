using Microsoft.AspNetCore.Mvc;
using Prescriptions.API.Services;

namespace Prescriptions.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPrescriptionService _svc;
        public PatientsController(IPrescriptionService svc) => _svc = svc;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var dto = await _svc.GetPatientDetailsAsync(id);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
using ApiIntegrationService.Data;
using ApiIntegrationService.Security;
using Microsoft.AspNetCore.Mvc;

namespace ApiIntegrationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("freshdesk")]
        public IActionResult GetFreshdeskData()
        {
            return Ok(_context.freshdeskcustomerscontacts.ToList());
        }

        [HttpGet("chargebee")]
        public IActionResult GetChargebeeData()
        {
            return Ok(_context.ChargebeeCustomers.ToList());
        }
        [HttpPost("synccustomerdata")]
        
        public IActionResult SyncCustomerData()
        {
            return Ok(_context.CustomerContacts.ToList());
        }
    }
}

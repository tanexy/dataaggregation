using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiIntegrationService.Data;
using ApiIntegrationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiIntegrationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdatedCustomerDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UpdatedCustomerDataController(AppDbContext _context)
        {
            this._context = _context;

        }
        [HttpGet("updatedcustomers")]
        public IActionResult GetUpdatedCustomers()
        {
            var updatedCustomers = _context.UpdatedContacts.ToList();
            return Ok(updatedCustomers);
        }


    }
}

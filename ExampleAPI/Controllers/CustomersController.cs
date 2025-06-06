using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExampleAPI.Models;

namespace ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ModelContext _context;

        public CustomersController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(customer);
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            var c = await _context.Customers.FirstOrDefaultAsync(x => x.Email.ToLower().Contains(customer.Email.ToLower()));
            if (c != null)
            {
                return BadRequest("Customers email was exist");
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not exist");
            }
            var orders = await _context.Orders.Where(x => x.CustomerId == id).ToListAsync();
            if (orders.Count > 0)
            {
                return BadRequest("This customer already had order, please deleted that order first");
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok("Delete success");
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}

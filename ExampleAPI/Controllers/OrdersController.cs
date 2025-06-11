using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExampleAPI.Models;
using ExampleAPI.Models.DTO;

namespace ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ModelContext _context;

        public OrdersController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders.Include(o => o.Customer).Select(x => new  { x.OrderId, x.CustomerId, CustomerName = x.Customer.FullName,x.OrderDate, x.TotalAmount, Address = x.Customer.Address}).ToListAsync();
            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDTO orderDTO)
        {
            if (id != orderDTO.OrderId)
            {
                return BadRequest("Id not match");
            }
            
            var order = new Order() {OrderId = orderDTO.OrderId, CustomerId = orderDTO.CustomerId, OrderDate = orderDTO.OrderDate, TotalAmount = orderDTO.TotalAmount };
            _context.Update(order);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound("This order not exist");
                }
                else
                {
                    throw;
                }
            }

            return Ok(order);
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderDTO orderDTO)
        {
            var c = await _context.Customers.Include(x => x.Orders).FirstOrDefaultAsync(x => x.CustomerId == orderDTO.CustomerId);

            if (c != null)
            {
                var order = new Order() { OrderDate = orderDTO.OrderDate, TotalAmount = orderDTO.TotalAmount, Customer = c};
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(order);
            }
            return BadRequest("Customer not exist");
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }
            var orderItems = await _context.OrderItems.Where(x => x.OrderId == id).ToListAsync();
            if (orderItems.Count > 0)
            {
                return BadRequest("This order already had order items, please delete that order items first");
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok("Delete order success");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}

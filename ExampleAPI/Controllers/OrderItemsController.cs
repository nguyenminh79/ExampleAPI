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
    public class OrderItemsController : ControllerBase
    {
        private readonly ModelContext _context;

        public OrderItemsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/OrderItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            var orderItem = await _context.OrderItems.ToListAsync();
            return Ok(orderItem);
        }

        // GET: api/OrderItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);

            if (orderItem == null)
            {
                return NotFound("OrderItem not exits");
            }

            return Ok(orderItem);
        }

        // PUT: api/OrderItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItemDTO orderItemDTO)
        {
            if (id != orderItemDTO.OrderItemId)
            {
                return BadRequest("Id not match");
            }
            var o = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderItemDTO.OrderId);
            var p = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == orderItemDTO.ProductId);
            if (p == null)
            {
                return BadRequest("Product not exits!");
            }
            if (o == null)
            {
                return BadRequest("Order not exits");
            }
            var orderItem = new OrderItem() { OrderItemId = orderItemDTO.OrderItemId, OrderId = orderItemDTO.OrderId, ProductId = orderItemDTO.ProductId, Quantity = orderItemDTO.Quantity, UnitPrice = orderItemDTO.UnitPrice, Order = o, Product = p };
            _context.Update(orderItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            o.TotalAmount = await _context.OrderItems.Where(x => x.OrderId == orderItemDTO.OrderId).Select(x => x.UnitPrice * x.Quantity).SumAsync();
            _context.Orders.Update(o);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/OrderItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItemDTO orderItemDTO)
        {
            var o = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderItemDTO.OrderId);

            var p = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == orderItemDTO.ProductId);
            if (p == null)
            {
                return BadRequest("Product not exits!");
            }
            if (o == null)
            {
                return BadRequest("Order not exits");
            }
            var orderItem = new OrderItem() { OrderId = orderItemDTO.OrderId, ProductId = orderItemDTO.ProductId, Quantity = orderItemDTO.Quantity, UnitPrice = orderItemDTO.UnitPrice };
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            //o.TotalAmount = await _context.OrderItems.Where(x => x.OrderId == orderItemDTO.OrderId).Select(x => x.UnitPrice * x.Quantity).SumAsync();
            //_context.Orders.Update(o);
            //await _context.SaveChangesAsync();

            return Ok(orderItem);
        }

        // DELETE: api/OrderItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            var o = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderItem.OrderId);
            _context.OrderItems.Remove(orderItem);

            await _context.SaveChangesAsync();
            o.TotalAmount = await _context.OrderItems.Where(x => x.OrderId == orderItem.OrderId).Select(x => x.UnitPrice * x.Quantity).SumAsync();
            _context.Orders.Update(o);
            return NoContent();
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.OrderItemId == id);
        }
    }
}

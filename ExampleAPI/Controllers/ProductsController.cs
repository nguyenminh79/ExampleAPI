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
    public class ProductsController : ControllerBase
    {
        private readonly ModelContext _context;

        public ProductsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("Id not match");
            }

            _context.Update(product);
            _context.OrderItems.Where(x => x.ProductId == product.ProductId).ToList();
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound("This product not exist");
                }
                else
                {
                    throw;
                }
            }

            return Ok(product);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("This product not exist");
            }
            var orderItems = await _context.OrderItems.Where(x => x.ProductId == id).ToListAsync();
            if (orderItems.Count > 0)
            {
                return BadRequest("This product already had order item, please delete that order item first");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Delete product success");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}

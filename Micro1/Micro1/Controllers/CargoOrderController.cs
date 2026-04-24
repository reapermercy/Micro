using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CargoTransportApi.Models;

namespace CargoTransportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoOrdersController : ControllerBase
    {
        private readonly CargoContext _context;

        public CargoOrdersController(CargoContext context)
        {
            _context = context;
        }

        // GET: api/CargoOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CargoOrder>>> GetCargoOrders()
        {
            return await _context.CargoOrders.ToListAsync();
        }

        // GET: api/CargoOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CargoOrder>> GetCargoOrder(int id)
        {
            var order = await _context.CargoOrders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/CargoOrders
        [HttpPost]
        public async Task<ActionResult<CargoOrder>> PostCargoOrder(CargoOrder order)
        {
            _context.CargoOrders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCargoOrder), new { id = order.Id }, order);
        }

        // PUT: api/CargoOrders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCargoOrder(int id, CargoOrder order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CargoOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CargoOrders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCargoOrder(int id)
        {
            var order = await _context.CargoOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.CargoOrders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CargoOrderExists(int id)
        {
            return _context.CargoOrders.Any(e => e.Id == id);
        }
    }
}
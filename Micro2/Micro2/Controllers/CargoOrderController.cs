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
        private const int MaxLimit = 1000;

        public CargoOrdersController(CargoContext context)
        {
            _context = context;
        }

        // GET: api/CargoOrders
        // При превышении 1000 элементов — возвращает случайное подмножество
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CargoOrder>>> GetCargoOrders()
        {
            var count = await _context.CargoOrders.CountAsync();

            if (count > MaxLimit)
            {
                var randomOrders = await _context.CargoOrders
                    .OrderBy(o => Guid.NewGuid())
                    .Take(MaxLimit)
                    .ToListAsync();
                return randomOrders;
            }

            return await _context.CargoOrders.ToListAsync();
        }

        // GET: api/CargoOrders/filter?status=В пути&originCity=Москва
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<CargoOrder>>> FilterCargoOrders(
            [FromQuery] string? status,
            [FromQuery] string? originCity,
            [FromQuery] string? destinationCity,
            [FromQuery] DateTime? shipmentDateFrom,
            [FromQuery] DateTime? shipmentDateTo)
        {
            var query = _context.CargoOrders.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrEmpty(originCity))
                query = query.Where(o => o.OriginCity == originCity);

            if (!string.IsNullOrEmpty(destinationCity))
                query = query.Where(o => o.DestinationCity == destinationCity);

            if (shipmentDateFrom.HasValue)
                query = query.Where(o => o.ShipmentDate >= shipmentDateFrom.Value);

            if (shipmentDateTo.HasValue)
                query = query.Where(o => o.ShipmentDate <= shipmentDateTo.Value);

            return await query.ToListAsync();
        }

        // GET: api/CargoOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CargoOrder>> GetCargoOrder(int id)
        {
            var order = await _context.CargoOrders.FindAsync(id);

            if (order == null)
                return NotFound();

            return order;
        }

        // POST: api/CargoOrders
        [HttpPost]
        public async Task<ActionResult<CargoOrder>> PostCargoOrder(CargoOrderDto dto)
        {
            var order = new CargoOrder
            {
                SenderName = dto.SenderName,
                ReceiverName = dto.ReceiverName,
                OriginCity = dto.OriginCity,
                DestinationCity = dto.DestinationCity,
                Weight = dto.Weight,
                CargoDescription = dto.CargoDescription,
                ShipmentDate = dto.ShipmentDate,
                Status = dto.Status
            };

            _context.CargoOrders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCargoOrder), new { id = order.Id }, order);
        }

        // PUT: api/CargoOrders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCargoOrder(int id, CargoOrderDto dto)
        {
            var order = await _context.CargoOrders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.SenderName = dto.SenderName;
            order.ReceiverName = dto.ReceiverName;
            order.OriginCity = dto.OriginCity;
            order.DestinationCity = dto.DestinationCity;
            order.Weight = dto.Weight;
            order.CargoDescription = dto.CargoDescription;
            order.ShipmentDate = dto.ShipmentDate;
            order.Status = dto.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CargoOrderExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/CargoOrders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCargoOrder(int id)
        {
            var order = await _context.CargoOrders.FindAsync(id);
            if (order == null)
                return NotFound();

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

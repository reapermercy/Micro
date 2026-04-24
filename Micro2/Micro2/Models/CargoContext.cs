using Microsoft.EntityFrameworkCore;

namespace CargoTransportApi.Models
{
    public class CargoContext : DbContext
    {
        public CargoContext(DbContextOptions<CargoContext> options) : base(options)
        {}
        public DbSet<CargoOrder> CargoOrders { get; set; } = null!;
    }
}
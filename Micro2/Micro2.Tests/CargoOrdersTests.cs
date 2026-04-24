using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CargoTransportApi.Models;
using Xunit;

namespace CargoTransportApi.Tests
{
    public class CargoOrdersTests
    {
        private HttpClient CreateClient()
        {
            var dbName = "TestDb_" + Guid.NewGuid();
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptors = services.Where(d =>
                            d.ServiceType == typeof(DbContextOptions<CargoContext>) ||
                            d.ServiceType == typeof(CargoContext) ||
                            d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                            .ToList();

                        foreach (var d in descriptors)
                            services.Remove(d);

                        services.AddDbContext<CargoContext>(options =>
                            options.UseInMemoryDatabase(dbName));
                    });
                });
            return factory.CreateClient();
        }

        private CargoOrderDto MakeOrder(int i) => new CargoOrderDto
        {
            SenderName = $"Отправитель {i}",
            ReceiverName = $"Получатель {i}",
            OriginCity = "Москва",
            DestinationCity = "Пенза",
            Weight = 10.0 + i,
            CargoDescription = $"Груз {i}",
            ShipmentDate = DateTime.UtcNow.AddDays(i % 30),
            Status = "Создан"
        };

        // Тест 1: добавление 100 элементов
        [Fact]
        public async Task Add100Orders_ShouldSucceed()
        {
            var client = CreateClient();

            for (int i = 0; i < 100; i++)
            {
                var response = await client.PostAsJsonAsync("/api/CargoOrders", MakeOrder(i));
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }

            var all = await client.GetFromJsonAsync<List<CargoOrder>>("/api/CargoOrders");
            Assert.NotNull(all);
            Assert.Equal(100, all.Count);
        }

        // Тест 2: добавление 100 000 элементов
        [Fact]
        public async Task Add100000Orders_ShouldSucceed()
        {
            var client = CreateClient();

            var tasks = Enumerable.Range(0, 100_000)
                .Select(i => client.PostAsJsonAsync("/api/CargoOrders", MakeOrder(i)));

            var responses = await Task.WhenAll(tasks);

            Assert.All(responses, r =>
                Assert.Equal(HttpStatusCode.Created, r.StatusCode));
        }

        // Тест 3: удаление всех элементов
        [Fact]
        public async Task DeleteAllOrders_ShouldSucceed()
        {
            var client = CreateClient();

            for (int i = 0; i < 5; i++)
                await client.PostAsJsonAsync("/api/CargoOrders", MakeOrder(i));

            var all = await client.GetFromJsonAsync<List<CargoOrder>>("/api/CargoOrders");
            Assert.NotNull(all);

            foreach (var order in all)
            {
                var response = await client.DeleteAsync($"/api/CargoOrders/{order.Id}");
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }

            var remaining = await client.GetFromJsonAsync<List<CargoOrder>>("/api/CargoOrders");
            Assert.NotNull(remaining);
            Assert.Empty(remaining);
        }
    }
}
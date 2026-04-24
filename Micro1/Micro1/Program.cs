using Microsoft.EntityFrameworkCore;
using CargoTransportApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<CargoContext>(opt =>
    opt.UseInMemoryDatabase("CargoOrderList"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CargoContext>();

    context.CargoOrders.AddRange(
        new CargoOrder
        {
            SenderName = "Иванов Иван Иванович",
            ReceiverName = "Петров Пётр Петрович",
            OriginCity = "Москва",
            DestinationCity = "Пенза",
            Weight = 150.5,
            CargoDescription = "Электроника",
            ShipmentDate = DateTime.Now.AddDays(2),
            Status = "Создан"
        },
        new CargoOrder
        {
            SenderName = "Сидоров Сергей Сергеевич",
            ReceiverName = "Козлов Константин Константинович",
            OriginCity = "Санкт-Петербург",
            DestinationCity = "Казань",
            Weight = 500.0,
            CargoDescription = "Строительные материалы",
            ShipmentDate = DateTime.Now.AddDays(-1),
            Status = "В пути"
        },
        new CargoOrder
        {
            SenderName = "Морозова Анна Владимировна",
            ReceiverName = "Лебедев Дмитрий Александрович",
            OriginCity = "Нижний Новгород",
            DestinationCity = "Самара",
            Weight = 75.0,
            CargoDescription = "Офисная мебель",
            ShipmentDate = DateTime.Now.AddDays(-5),
            Status = "Доставлен"
        }
    );

    context.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
using Api.Services;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using Api.Data;
using Api.Models;
using Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default");
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "vetdemo.db");
var connString = cs!.Replace("vetdemo.db", dbPath);

builder.Services.AddDbContext<VetAppDbContext>(options =>
    options.UseSqlite(connString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddScoped<IEmailService, MockEmailService>();
builder.Services.AddScoped(typeof(IDBRepository<>), typeof(DBRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

await PopulateInitialDbValues(app);

app.Run();

static async Task PopulateInitialDbValues(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<VetAppDbContext>();
        await db.Database.EnsureCreatedAsync();

        if (!await db.Veterinarians.AnyAsync())
        {
            var vet1 = new Veterinarian { Id = new Guid("70a5e606-25c9-4412-907d-e490bee7bfce"), Name = "Vet1" };
            var vet2 = new Veterinarian { Id = new Guid("7399388f-3134-4381-951a-de10a9895bd0"), Name = "Vet2" };
            var custDog = new Customer { Id = new Guid("5f4983b1-9a1f-4e1e-941f-31c8c5c8587b"), Name = "Dog Owner", Email = "dogowner@example.com" };
            var custCat = new Customer { Id = new Guid("11dadc38-d80b-4a22-8bd4-e91a7e91775a"), Name = "Cat Owner", Email = "catowner@example.com" };
            var custRabbit = new Customer { Id = new Guid("f0928298-20c1-450c-a110-b432776c0b35"), Name = "Rabbit Owner", Email = "rabbitsowner@example.com" };
            var dog = new Animal
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                Name = "Dog",
                CustomerId = custDog.Id,
                BirthDate = DateTime.Now.AddYears(-3)
            };
            var cat = new Animal
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d477"),
                Name = "Cat",
                CustomerId = custCat.Id,
                BirthDate = DateTime.Now.AddYears(-2)
            };
            var rabbit = new Animal
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d476"),
                Name = "Rabbit",
                CustomerId = custRabbit.Id,
                BirthDate = DateTime.Now.AddYears(-1)
            };
            var appt1 = new Appointment
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                AnimalId = dog.Id,
                CustomerId = custDog.Id,
                VeterinarianId = vet1.Id,
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                Status = AppointmentStatus.Scheduled,
                Notes = "Vet appointment",
            };
            var appt2 = new Appointment
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d480"),
                AnimalId = dog.Id,
                CustomerId = custDog.Id,
                VeterinarianId = vet1.Id,
                StartTime = DateTime.UtcNow.AddDays(2),
                EndTime = DateTime.UtcNow.AddDays(2).AddHours(1),
                Status = AppointmentStatus.Scheduled,
                Notes = "Follow-up check",
            };
            var appt3 = new Appointment
            {
                Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d481"),
                AnimalId = cat.Id,
                CustomerId = custCat.Id,
                VeterinarianId = vet2.Id,
                StartTime = DateTime.UtcNow.AddDays(2),
                EndTime = DateTime.UtcNow.AddDays(2).AddHours(1),
                Status = AppointmentStatus.Scheduled,
            };

            db.Veterinarians.AddRange(new List<Veterinarian> { vet1, vet2 });
            db.Customers.AddRange(new List<Customer> { custDog, custCat, custRabbit });
            db.Animals.AddRange(new List<Animal> { dog, cat, rabbit });
            db.Appointments.AddRange(new List<Appointment> { appt1, appt2, appt3 });

            await db.SaveChangesAsync();
        }
    }
}
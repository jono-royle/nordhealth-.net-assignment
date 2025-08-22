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

        //If db is empty, populate
        if(!await db.Veterinarians.AnyAsync())
        {
            db.Veterinarians.AddRange(VetData.Vets);
            db.Customers.AddRange(CustomerData.Customers);
            db.Animals.AddRange(AnimalData.Animals);
            db.Appointments.AddRange(AppointmentData.Appointments);

            await db.SaveChangesAsync();
        }
    }
}
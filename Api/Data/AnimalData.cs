using Api.Models;

namespace Api.Data;

internal static class AnimalData
{
    internal static List<Animal> Animals = new()
    {
        new Animal
        {
            Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            Name = "Dog",
            BirthDate = DateTime.Now.AddYears(-3),
            CustomerId = CustomerData.Customers.First().Id
        },
        new Animal
        {
            Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d477"),
            Name = "Cat",
            BirthDate = DateTime.Now.AddYears(-2),
            CustomerId = CustomerData.Customers[1].Id
        },
        new Animal
        {
            Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d476"),
            Name = "Rabbit",
            BirthDate = DateTime.Now.AddYears(-1),
            CustomerId = CustomerData.Customers[2].Id
        }
    };
}
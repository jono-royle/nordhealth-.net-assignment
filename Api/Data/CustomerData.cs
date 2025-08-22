using Api.Models;

namespace Api.Data
{
    internal static class CustomerData
    {
        internal static List<Customer> Customers = new()
        {
            new Customer
            {
                Id = new Guid("5f4983b1-9a1f-4e1e-941f-31c8c5c8587b"),
                Name = "Dog Owner",
                Email = "dogowner@example.com"
            },
            new Customer { Id = new Guid("11dadc38-d80b-4a22-8bd4-e91a7e91775a"), Name = "Cat Owner", Email = "catowner@example.com" },
            new Customer { Id = new Guid("f0928298-20c1-450c-a110-b432776c0b35"), Name = "Rabbit Owner", Email = "rabbitsowner@example.com" }
        };
    }
}

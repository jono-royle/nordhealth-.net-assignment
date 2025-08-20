using System.Text.Json.Serialization;

namespace Api.Models;

public class Animal
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public Guid CustomerId { get; set; }

    [JsonIgnore]
    public Customer? Owner { get; set; }

    [JsonIgnore]
    public List<Appointment>? Appointments { get; set; }
}
using System.Text.Json.Serialization;

namespace Api.Models;

public enum AppointmentStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

public class Appointment
{
    [JsonIgnore]
    public Guid Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Guid AnimalId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid VeterinarianId { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public string? Notes { get; set; }

    [JsonIgnore]
    public Animal? Animal { get; set; }
    [JsonIgnore]
    public Customer? Customer { get; set; }
    [JsonIgnore]
    public Veterinarian? Veterinarian { get; set; }
}
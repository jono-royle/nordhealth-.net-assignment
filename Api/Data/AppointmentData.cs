using Api.Models;

namespace Api.Data;

internal static class AppointmentData
{
    internal static List<Appointment> Appointments = new()
    {
        new Appointment
        {
            Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            AnimalId = AnimalData.Animals.First().Id,
            CustomerId = AnimalData.Animals.First().CustomerId,
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(1),
            Notes = "Vet appointment",
            VeterinarianId = new Guid("70a5e606-25c9-4412-907d-e490bee7bfce"),
            Status = AppointmentStatus.Scheduled
        },
        new Appointment
        {
            Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d480"),
            AnimalId = AnimalData.Animals.First().Id,
            CustomerId = AnimalData.Animals.First().CustomerId,
            Notes = "Follow-up check",
            StartTime = DateTime.Now.AddDays(2),
            EndTime = DateTime.Now.AddDays(2).AddHours(1),
            VeterinarianId = new Guid("70a5e606-25c9-4412-907d-e490bee7bfce"),
            Status = AppointmentStatus.Scheduled
        },
        new Appointment
        {
            Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d481"),
            AnimalId = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d477"),
            CustomerId = AnimalData.Animals.FirstOrDefault(a => a.Id == new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d477")).CustomerId,
            Notes = "Follow-up check",
            StartTime = DateTime.Now.AddDays(2),
            EndTime = DateTime.Now.AddDays(2).AddHours(1),
            VeterinarianId = new Guid("7399388f-3134-4381-951a-de10a9895bd0"),
            Status = AppointmentStatus.Scheduled
        },
    };
}
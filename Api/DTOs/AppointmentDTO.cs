using Api.Models;

namespace Api.DTOs
{
    public class AppointmentDTO
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? AnimalName { get; set; }
        public string? OwnerName { get; set; }
        public string? VetName { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
    }
}

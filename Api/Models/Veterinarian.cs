namespace Api.Models
{
    public class Veterinarian
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

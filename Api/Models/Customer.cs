namespace Api.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

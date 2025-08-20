namespace Api.DTOs
{
    public class AnimalDTO
    {
        public string? Name { get; set; }

        public DateTime BirthDate { get; set; }

        public Guid CustomerId { get; set; }
    }
}

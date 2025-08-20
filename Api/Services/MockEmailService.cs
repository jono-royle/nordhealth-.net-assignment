
namespace Api.Services
{
    public class MockEmailService : IEmailService
    {
        public Task SendEmailAsync(string email)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Email sent to {email}");
            });
        }
    }
}

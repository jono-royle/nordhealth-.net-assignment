namespace Api.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email);
    }
}

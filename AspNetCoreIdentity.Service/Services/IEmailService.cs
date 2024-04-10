namespace AspNetCoreIdentity.Service.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(string resetLink, string to);
    }
}

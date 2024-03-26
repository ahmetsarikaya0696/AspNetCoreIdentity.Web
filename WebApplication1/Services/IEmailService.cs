namespace AspNetCoreIdentity.Web.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(string resetLink, string to);
    }
}

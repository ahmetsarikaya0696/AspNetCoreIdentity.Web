
using AspNetCoreIdentity.Web.OptionsModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;

namespace AspNetCoreIdentity.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendResetPasswordEmailAsync(string resetLink, string to)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("localhost.com", _emailSettings.Email));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = "Şifre Sıfırlama";

            var bodyBuilder = new BodyBuilder()
            {
                HtmlBody = $@"<h4>Şifrenizi yenilemek için aşağıdaki linke tıklayınız</h4>
                              <p>
                                <a href='{resetLink}'>Şifreyi Sıfırla</a>
                              </p>               
                             "
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);

                await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}

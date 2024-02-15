using ETicaretAPI.Application.Abstractions.Services.Mail;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infastructure.Services.Mail
{
    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new()
            {
                IsBodyHtml = isBodyHtml,
                Subject = subject,
                Body = body,
                From = new(_configuration["Mail:Username"], "E-Commerce Test", Encoding.UTF8),
            };

            foreach (var item in tos)
            {
                mail.To.Add(item);
            }

            SmtpClient smtpClient = new()
            {
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                Port = 587,
                EnableSsl = true,
                Host = _configuration["Mail:Host"]
            };

            await smtpClient.SendMailAsync(mail);
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder resetMail = new StringBuilder();
            resetMail.AppendLine("Merhaba<br>Eğer yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz! <br><strong><a target=\"_blank\" href=\"");
            resetMail.AppendLine(_configuration["AngularClientUrl"]);
            resetMail.AppendLine("/update-password/");
            resetMail.AppendLine(userId);
            resetMail.AppendLine("/");
            resetMail.AppendLine(resetToken);
            resetMail.AppendLine("\"> Yeni şifre talebi için tıklayınız </a></strong><br><br><span style=\"font-size:12px;\"> NOT: Şifre yenileme talebinde bulunmadıysanız bu maili ciddiye almayınız! </span> <br>");
            await SendMailAsync(to, "Şifre Yenileme Talebi", resetMail.ToString());
        }

        public async Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, string userName, string userNameSurname)
        {
            string mail = $"Sayın {userNameSurname} Merhaba, <br>" +
                $"{orderDate} tarihinde vermiş olduğunuz {orderCode} kodlu siparişiniz tamamlanmış ve kargolanmıştır!";

            await SendMailAsync(to, $" {orderCode} Siparişiniz Kargolandı", mail);
        }
    }
}

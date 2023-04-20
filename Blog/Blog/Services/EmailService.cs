using System.Net;
using System.Net.Mail;

namespace Blog.Services
{
    public class EmailService
    {
        private static string API_KEY_SENDGRID = Environment.GetEnvironmentVariable("API_KEY_SENDGRID", EnvironmentVariableTarget.Machine);

        public bool Send(
            string toName,
            string toEmail,
            string subject,
            string body,
            string fromName = "teste marcio",
            string fromEmail = "marciodeath@hotmail.com")
        {
            var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);
            smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.UserName, API_KEY_SENDGRID);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            var mail = new MailMessage();
            mail.From = new MailAddress(fromEmail, fromName);
            mail.To.Add(new MailAddress(toEmail, toName));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

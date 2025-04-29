using System.Threading.Tasks;

namespace ScreenGIFCapture.Settings
{
    using System.Net.Mail;
    using System.Net;

    public class Mailer
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly bool _enableSsl;

        public Mailer(string smtpServer, int port, string senderEmail, string senderPassword, bool enableSsl = true)
        {
            _smtpServer = smtpServer;
            _port = port;
            _senderEmail = senderEmail;
            _senderPassword = senderPassword;
            _enableSsl = enableSsl;
        }

        public async Task SendEmail(string recipientEmail, string subject, string body, string attachmentPath = null)
        {
            using (var client = new SmtpClient(_smtpServer, _port))
            {
                client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                client.EnableSsl = _enableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = body
                };
                mailMessage.To.Add(recipientEmail);

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    mailMessage.Attachments.Add(new Attachment(attachmentPath));
                }

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}

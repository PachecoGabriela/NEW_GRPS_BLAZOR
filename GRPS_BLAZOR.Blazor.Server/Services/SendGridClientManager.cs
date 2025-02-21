using DevExpress.ExpressApp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;


namespace GRPS_BLAZOR.Blazor.Server.Services
{
    public class SendGridClientManager
    {
        private static SendGridClientManager _instance;
        private static readonly object _lock = new object();
        private SendGridClient _client;
        public IConfiguration configuration { get; set; }

        // SendGrid API Key
        private string ApiKey { get; set; } = "EMPTY";

        // Constructor is private so it can't be instantiated outside of this class
        private SendGridClientManager(IConfiguration configuration)
        {
            if (!ModuleHelper.IsDesignMode)
            {
                ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.User) ?? configuration["SENDGRID:API_KEY"] ?? "";

            }
            else
            {
                ApiKey = "EMPTY";
            }
            _client = new SendGridClient(ApiKey);
        }

        // The public instance property to access the singleton instance
        public static SendGridClientManager GetInstance(IConfiguration configuration)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SendGridClientManager(configuration);
                }
                return _instance;
            }
        }

        public SendGridClient Client => _client;

        public async Task<bool> SendEmail(string to, string htmlContent, string subject, List<(string FileName, byte[] Content, string MimeType)> attachments = null)
        {
            SendGridMessage msg = new SendGridMessage();
            EmailAddress fromAddress = new EmailAddress("dev@xari.io");
            List<EmailAddress> recipients = to
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(email => new EmailAddress(email.Trim(), email.Trim()))
            .ToList();

            if (recipients.Count == 0)
            {
                return false; // No hay destinatarios válidos
            }

            msg.SetSubject(subject);
            msg.SetFrom(fromAddress);
            msg.AddTos(recipients);
            msg.HtmlContent = htmlContent ?? "";

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    string base64Content = Convert.ToBase64String(attachment.Content);
                    msg.AddAttachment(attachment.FileName, base64Content, attachment.MimeType);
                }
            }

            Response response = await _client.SendEmailAsync(msg);
            if (Convert.ToInt32(response.StatusCode) >= 400)
            {
                return false;
            }
            return true;
        }
    }
}

using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Mailing.API.Smtp;

public interface ISmtpSender
{
    public void Send(string userId, string username, string email, string subject, string content);
}

public class MailDevSmtpSender : ISmtpSender
{
    private readonly SmtpOptions _smtpOptions;
    private readonly string _clientUrl;
    private readonly string _unsubInfo;
    private readonly string _bestWishesInfo;
    private readonly string _userGreetings;
    private readonly ILogger<MailDevSmtpSender> _logger;

    public MailDevSmtpSender(IOptions<SmtpOptions> smtpOptions,
        IConfiguration configuration,
        ILogger<MailDevSmtpSender> logger)
    {
        _logger = logger;
        _smtpOptions = smtpOptions?.Value ?? throw new ArgumentNullException(nameof(smtpOptions));
        _clientUrl = configuration.GetValue<string>("ClientUrl");
        _unsubInfo = configuration.GetValue<string>("UnsubInfo");
        _bestWishesInfo = configuration.GetValue<string>("BestWishesInfo");
        _userGreetings = configuration.GetValue<string>("UserGreetings");
    }

    public void Send(string userId, string username, string email, string subject, string content)
    {
        if (!IsInputValid(userId, username, email, subject, content))
            return;

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpOptions.ServiceName, _smtpOptions.ServiceMail));
            message.To.Add(new MailboxAddress(username, email));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Plain)
            {
                Text = BuildServiceMessageWithContent(userId, username, content)
            };

            using var smtpClient = new SmtpClient();
            smtpClient.Connect(_smtpOptions.Host, _smtpOptions.Port, SecureSocketOptions.None);
            smtpClient.Send(message);
            smtpClient.Disconnect(true);
            _logger.LogInformation("{Message} {Username} {Email} {Subject}", "Message was sent to the user", username,
                email, subject);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message} {Username} {Email} {Subject}", "The message sending was failed", username,
                email, subject);
        }
    }

    private string BuildServiceMessageWithContent(string userId, string username, string content)
    {
        var sb = new StringBuilder();
        sb.AppendFormat(_userGreetings, username);
        sb.AppendLine("\n");
        sb.AppendLine(content);
        sb.AppendLine(_bestWishesInfo);
        sb.AppendFormat(_unsubInfo, _clientUrl, userId);
        return sb.ToString();
    }

    private static bool IsInputValid(string userId, string username, string email, string subject, string content)
    {
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(subject) &&
               !string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(userId);
    }
}
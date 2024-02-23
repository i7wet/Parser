using System.Net.Mail;

namespace EmailService;

public class EmailService
{
    private const string EMAIL_SENDER_ADDRESS = "emailservice145@gmail.com";
    private SmtpClient _client; 
    public EmailService()
    {
        _client = CreateClient();
    }
    
    private  SmtpClient CreateClient()
    {
        SmtpClient client = new SmtpClient();
        client.Host = "smtp.gmail.com";
        client.Port = 587;
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        var credentials = new System.Net.NetworkCredential(EMAIL_SENDER_ADDRESS, "nnzp yaix fxiw oytw");
        client.Credentials = credentials;
        return client;
    }

    public async Task SendMessage(string to, string message)
    {
        try
        {
            MailAddress from = new MailAddress(EMAIL_SENDER_ADDRESS, "TestApp");
            MailAddress two = new MailAddress(to);
            MailMessage m = new MailMessage(from, two);
            m.Subject = "Изменение цен";
            m.Body = message;
            await _client.SendMailAsync(m);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Не удалось отпарвить письмо. Почта - {to}.");
            Console.WriteLine($"{e.Message}");
        }
    }
    
}
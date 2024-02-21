using System.Net.Mail;

namespace EmailService;

public class Client
{
    public static SmtpClient Create(string emailSenderAddress)
    {
        SmtpClient client = new SmtpClient();
        client.Host = "smtp.gmail.com";
        client.Port = 587;
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        var credentials = new System.Net.NetworkCredential(emailSenderAddress, "nnzp yaix fxiw oytw");
        client.Credentials = credentials;
        return client;
    }
}
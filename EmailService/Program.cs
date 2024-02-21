using System.Net.Mail;
using EmailService;
using Redis;


var redis = new RedisDb();
var emailSenderAddress = "emailservice145@gmail.com";
var client = Client.Create(emailSenderAddress);

while (true)
{
    var data = await redis.GetMessageAsync();
    if (data.IsSome)
    {
        var unwrapData = data.Unwrap();
        var message = $"У квартиры {unwrapData.UrlApartment} новая цена => {unwrapData.Price}";
        await SendMessage(unwrapData.Email, message);
    }
}

async Task SendMessage(string to, string message)
{
    try
    {
        MailAddress from = new MailAddress(emailSenderAddress, "TestApp");
        MailAddress two = new MailAddress(to);
        MailMessage m = new MailMessage(from, two);
        m.Subject = "Изменение цен";
        m.Body = message;
        await client.SendMailAsync(m);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Не удалось отпарвить письмо. Почта - {to}.");
        Console.WriteLine($"{e.Message}");
    }
}
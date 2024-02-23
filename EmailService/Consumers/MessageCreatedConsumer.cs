using MassTransit;
using RabbitMQ.Models;

namespace EmailService.Consumers;

public class MessageCreatedConsumer : IConsumer<Message>
{
    private readonly EmailService _emailService;

    public MessageCreatedConsumer(EmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<Message> context)
    {
        string message = $"У квартиры {context.Message.UrlApartment} новая цена => {context.Message.Price}";
        await _emailService.SendMessage(context.Message.Email, message);
    }
    
}
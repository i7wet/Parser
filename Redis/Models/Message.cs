namespace Redis.Models;

public class Message
{
    public Message( string email, string urlApartment, decimal price)
    {
        Email = email;
        UrlApartment = urlApartment;
        Price = price;
    }

    public Guid Id { get; set; } = new Guid();
    public string Email { get; set; }
    public string UrlApartment { get; set; }
    public decimal Price { get; set; }
}
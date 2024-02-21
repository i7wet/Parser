using Microsoft.EntityFrameworkCore;

namespace DbContext.Database.Models;

[Index(nameof(Url), IsUnique = true)]
public class ApartmentDb
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public decimal Price { get; set; }
    
}
using Microsoft.EntityFrameworkCore;

namespace DbContext.Database.Models;

[Index(nameof(Email), IsUnique = true)]
public class SubscriberDb
{
    public Guid Id { get; set; }
    public List<ApartmentDb> Apartments { get; set; }
    public string Email { get; set; }
}
using System.Text.Json.Serialization;

namespace test.DTO;

public class ApartmentDTO
{
    [JsonConstructor]
    public ApartmentDTO() { }
    
    public string? Url { get; set; }
    public decimal? Price { get; set; }
}
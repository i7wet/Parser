using System.Text.Json;
using DbContext.Database;
using DbContext.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using test.DTO;

namespace test.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly TestDbContext _testDbContext;
    private readonly HttpClient _httpClient;


    public TestController(ILogger<TestController> logger,
        TestDbContext testDbContext, HttpClient httpClient)
    {
        _logger = logger;
        _testDbContext = testDbContext;
        _httpClient = httpClient;
    }

    [HttpGet]
    public async Task<IResult> Get([FromForm] string email)
    {
        var subscribersDb = await _testDbContext.Subscribers.Where(x => x.Email == email)
            .Include(subscriberDb => subscriberDb.Apartments).ToListAsync();
        if (subscribersDb.Count == 0)
            return Results.NotFound("Не найдено отслеживаемых квартир у указанного Email.");
        var subscriber = subscribersDb.Single();
        var apartments = subscriber.Apartments
            .Select(apartmentDb => new ApartmentDTO() { Price = apartmentDb.Price, Url = apartmentDb.Url }).ToList();
        return Results.Json(apartments);
    }

    [HttpPut]
    public async Task<IResult> Put([FromForm] string email, [FromForm] string url)
    {
        var subscribersDb = await _testDbContext.Subscribers.Where(x => x.Email == email)
            .Include(subscriberDb => subscriberDb.Apartments).ToListAsync();
        var apartmentsDb = await _testDbContext.Apartments.Where(x => x.Url == url).ToListAsync();
        ApartmentDb apartmentDb = new();
        SubscriberDb subscriberDb = new();
        if (subscribersDb.Count == 0)
        {
            if (apartmentsDb.IsNullOrEmpty())
            {
                apartmentDb = new ApartmentDb() { Id = new Guid(), Price = 0, Url = url };
                _testDbContext.Apartments.Add(apartmentDb);
            }
            else
            {
                apartmentDb = apartmentsDb.Single();
            }

            subscriberDb = new SubscriberDb()
                { Id = new Guid(), Email = email, Apartments = new() { apartmentDb } };
            _testDbContext.Subscribers.Add(subscriberDb);
        }
        else
        {
            subscriberDb = subscribersDb.Single();
            if (subscriberDb.Apartments.Exists(x => x.Url == url))
            {
                return Results.BadRequest($"{email} уже подписан на {url}");
            }

            if (apartmentsDb.IsNullOrEmpty())
            {
                apartmentDb = new ApartmentDb() { Id = new Guid(), Price = 0, Url = url };
                _testDbContext.Apartments.Add(apartmentDb);
            }
            else
            {
                apartmentDb = apartmentsDb.Single();
            }

            subscriberDb.Apartments.Add(apartmentDb);
            _testDbContext.Update(subscriberDb);
        }

        await _testDbContext.SaveChangesAsync();
        var apartmentJson = JsonSerializer.Serialize(apartmentDb);
        var subscriberJson = JsonSerializer.Serialize(subscriberDb);
        var jsonContent = new StringContent("{" + "\"Apartment\":"+ apartmentJson + "," + "\"Subscriber\":" + subscriberJson  + "}");
        await _httpClient.PostAsync("http://localhost:3000/", jsonContent);
        return Results.Ok();
    }
}
using System.Text.Json;
using DbContext.Database;
using DbContext.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Redis;
using Redis.Models;
using test.DTO;
using Utilities;

namespace test.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly TestDbContext _testDbContext;
    private readonly RedisDb _redisDb;
    private readonly IConverter _converter;


    public TestController(ILogger<TestController> logger,
        TestDbContext testDbContext, RedisDb redisDb, IConverter converter)
    {
        _logger = logger;
        _testDbContext = testDbContext;
        _redisDb = redisDb;
        _converter = converter;
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
        if(!ValidateApartmentUrl(url))
            return Results.BadRequest($"Ссылка указана не корректно");
        if (ValidateEmail(email))
            return Results.BadRequest($"Неверный Email");
        
        var subscribersDb = await _testDbContext.Subscribers.Where(x => x.Email == email)
            .Include(subscriberDb => subscriberDb.Apartments).ToListAsync();
        var apartmentsDb = await _testDbContext.Apartments.Where(x => x.Url == url).ToListAsync();
        ApartmentDb apartmentDb = new();
        SubscriberDb subscriberDb = new();
        var isNewApartment = false;
        var isNewSubscriber = false;
        
        if (subscribersDb.Count == 0)
        {
            if (apartmentsDb.IsNullOrEmpty())
            {
                apartmentDb = new ApartmentDb() { Id = new Guid(), Price = 0, Url = url };
                _testDbContext.Apartments.Add(apartmentDb);
                isNewApartment = true;
            }
            else
            {
                apartmentDb = apartmentsDb.Single();
            }

            subscriberDb = new SubscriberDb()
                { Id = new Guid(), Email = email, Apartments = new() { apartmentDb } };
            isNewSubscriber = true;
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
                isNewApartment = true;
            }
            else
            {
                apartmentDb = apartmentsDb.Single();
            }

            subscriberDb.Apartments.Add(apartmentDb);
            _testDbContext.Update(subscriberDb);
        }
        
        await _testDbContext.SaveChangesAsync();
        try
        {
            await WriteSubscribeInQueue(apartmentDb, subscriberDb);
        }
        catch (Exception e)
        {
            if (isNewApartment)
                _testDbContext.Apartments.Remove(apartmentDb);
            if(isNewSubscriber)
                _testDbContext.Apartments.Remove(apartmentDb);
            else if (isNewApartment == false && isNewSubscriber == false)
            {
                subscriberDb.Apartments.Remove(apartmentDb);
                _testDbContext.Update(subscriberDb);
            }
            await _testDbContext.SaveChangesAsync();
            return Results.Problem(detail: "Не удалось записать данные. Пожалуйста повторите попытку.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return Results.Ok();
    }

    private async Task WriteSubscribeInQueue(ApartmentDb apartmentDb, SubscriberDb subscriberDb)
    {
        var apartment = _converter.Convert<ApartmentDb, Apartment>(apartmentDb);
        var subscriber = _converter.Convert<SubscriberDb, Subscriber>(subscriberDb);
        var subscribe = new Subscribe() { Apartment = apartment, Subscriber = subscriber };
        await _redisDb.EnqueueSubscribeAsync(subscribe);
    }

    private bool ValidateEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false;
        }
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch {
            return false;
        }
    }

    private bool ValidateApartmentUrl(string url)
    {
        if (url.Contains("https://prinzip.su/apartments"))
            return true;
        return false;
    }
}
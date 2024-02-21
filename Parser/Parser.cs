using DbContext.Database.Models;
using PuppeteerSharp;
using Utilities;

namespace Parser;

public class Parser
{
    /// <summary>
    /// Парсит страницу.
    /// Обновляет поле <paramref name="apartment"/>.<see cref="ApartmentDb.Price"/>, если цена изменилась.
    /// </summary>
    /// <returns>True - если цена изменилась. False - цена  не изменилась.</returns>
    public static async Task<Result<bool, Exception>> Parse(ApartmentDb apartment)
    {
        decimal newPrice = 0;
        try
        { 
            await new BrowserFetcher().DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions 
            { 
                Headless = true
            }); 
            var page = await browser.NewPageAsync();
            await page.GoToAsync(apartment.Url, timeout: 0);
            var jsSelectAllPrices =
                @"Array.from(document.querySelectorAll('div[class=""flat-prices__block-current""]')).map(a => a.innerText)";
            await page.WaitForExpressionAsync(jsSelectAllPrices);
            var prices = await page.EvaluateExpressionAsync<string[]>(jsSelectAllPrices);
            var clearPrice = prices.Single().Replace(" ", "").Replace("\u20bd", "");
            newPrice = Convert.ToDecimal(clearPrice);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Не уадлось получить цену. Сылка на квартиру- {apartment.Url}");
            return exception;
        }
        
        if (newPrice != apartment.Price)
        {
            apartment.Price = newPrice;
            return true;
        }
        Console.WriteLine($"{apartment.Url}: цена не изменилась");
        

        return false;

    }
}
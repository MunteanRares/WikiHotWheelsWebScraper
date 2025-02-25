using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using WikiHotWheelsWebScraper.Models;
using WikiHotWheelsWebScraper.Services;

ScrapeHotWheelsWiki hwScraper = new ScrapeHotWheelsWiki();
await hwScraper.InitializeAsync();
List<int> allYears = hwScraper.GetAllAvailableYears();

//for (int i = allYears.First(); i <= allYears.Last(); i++)
//{
//    IHtmlDocument yearDoc = await hwScraper.ScrapeYearCollection(i);
//}

await hwScraper.InitializeAsync();
List<int> availableYears = hwScraper.GetAllAvailableYears();
for (int year = 1980; year <= availableYears.Last(); year++)
{
    List<HotWheelsModel> hotWheelsModels = await hwScraper.DefaultDataBasePopulation(year);
    foreach (HotWheelsModel car in hotWheelsModels)
    {
        Console.WriteLine(car.PhotoURL);
    }
}
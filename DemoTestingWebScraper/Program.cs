using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using WikiHotWheelsWebScraper.Services;

ScrapeHotWheelsWiki hwScraper = new ScrapeHotWheelsWiki();
List<int> allYears = hwScraper.GetAllAvailableYears();

//for (int i = allYears.First(); i <= allYears.Last(); i++)
//{
//    IHtmlDocument yearDoc = await hwScraper.ScrapeYearCollection(i);
//}

IHtmlDocument yearDoc = await hwScraper.ScrapeYearCollection(1978);

var carTables = yearDoc.QuerySelectorAll("table");
int carNameColumnIndex = -1;

foreach (var table in carTables.Take(carTables.Length - 1))
{
    var colNames = table.QuerySelectorAll("tr th");
    if (colNames.Length == 0)
    {
        colNames = table.QuerySelectorAll("tr td");
    }
    for (int i = 0; i < colNames.Length; i++)
    {
        switch (colNames[i].TextContent.Trim().ToLower())
        {
            case "casting":
            case "car name":
            case "name":
            case "casting name":
            case "model name":
                carNameColumnIndex = i;
                break;
        }
    }

    var rows = table.QuerySelectorAll("tbody tr");
    foreach (var row in rows.Skip(1))
    {
        var columns = row.Children;
        Console.WriteLine(columns[carNameColumnIndex].TextContent.Trim());
    }
}



//IHtmlDocument customYearDoc = await hwScraper.ScrapeYearCollection(2024);

//var tableWithCars = customYearDoc.QuerySelector("table");
//var body = tableWithCars.QuerySelector("tbody");
//var rows = body.QuerySelectorAll("tr");
//foreach (var row in rows)
//{
//    Console.WriteLine(row.TextContent);
//}






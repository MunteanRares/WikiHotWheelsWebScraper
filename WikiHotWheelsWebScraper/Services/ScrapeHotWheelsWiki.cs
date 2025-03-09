using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using WikiHotWheelsWebScraper.Models;

namespace WikiHotWheelsWebScraper.Services
{
    public class ScrapeHotWheelsWiki : IScrapeHotWheelsWiki
    {
        private readonly HttpClient _httpClient = new HttpClient();

        private async Task<IHtmlDocument> ScrapeMainPage()
        {

            HttpResponseMessage request = await _httpClient.GetAsync("https://hotwheels.fandom.com/wiki/Hot_Wheels");

            // Check if the response is successful
            Stream response = await request.Content.ReadAsStreamAsync();

            // Create a new HtmlParser object that will be used to parse the HTML content in a structured way
            HtmlParser parser = new HtmlParser();

            // IHtmlDocument is an interface that represents an HTML document with a DOM tree
            IHtmlDocument document = await parser.ParseDocumentAsync(response);
            return document;
        }

        private async Task<IHtmlDocument> ScrapeYearCollection(int year)
        {
            HttpResponseMessage request = await _httpClient.GetAsync($"https://hotwheels.fandom.com/wiki/List_of_{year}_Hot_Wheels");

            Stream response = await request.Content.ReadAsStreamAsync();

            HtmlParser parser = new HtmlParser();

            IHtmlDocument yearPage = await parser.ParseDocumentAsync(response);

            return yearPage;
        }

        public async Task<List<int>> GetAllAvailableYears()
        {
            IHtmlDocument doc = await ScrapeMainPage();

            var divWithYears = doc.QuerySelector(".wikitable.mw-collapisble.mw-collapsed tbody tr:last-child p");
            List<string> listOfString = new List<string>(divWithYears.TextContent.Split(" "));
            List<int> output = new List<int>();

            foreach (string str in listOfString)
            {
                if (int.TryParse(str, out int year))
                {
                    output.Add(year);
                }
            }

            return output;
        }

        public async Task<List<HotWheelsModel>> DefaultDataBasePopulation(int year)
        {
            IHtmlDocument yearDoc = await ScrapeYearCollection(year);
            var carTables = yearDoc.QuerySelectorAll("table");
            int carNameColumnIndex = -1;
            int seriesNameColumnIndex = -1;
            int photoUrlColumnIndex = -1;
            int collectionNumColumnIndex = -1;
            int seriesNumColumnIndex = -1;
            int toyColumnIndex = -1;

            HashSet<string> carNameColumns = new HashSet<string> { "casting", "carname", "name", "castingname", "modelname" };
            HashSet<string> seriesNameColumns = new HashSet<string> { "series", "seriesname", "seriessubset" };
            List<HotWheelsModel> hotWheelsModels = new List<HotWheelsModel>();

            foreach (var table in carTables.Take(carTables.Length - 1))
            {
                var colNames = table.QuerySelectorAll("tr th");
                if (colNames.Length == 0)
                {
                    colNames = table.QuerySelectorAll("tr td");
                }

                for (int i = 0; i < colNames.Length; i++)
                {
                    string columnName = string.Join("", colNames[i].TextContent.Trim().ToLower().Split(" "));
                    if (carNameColumns.Contains(columnName))
                    {
                        carNameColumnIndex = i;
                    }
                    else if (seriesNameColumns.Contains(columnName))
                    {
                        seriesNameColumnIndex = i;
                    }
                    else if (columnName == "photo")
                    {
                        photoUrlColumnIndex = i;
                    }
                    else if (columnName == "col.#" || columnName == "col#")
                    {
                        collectionNumColumnIndex = i;
                    }
                    else if (columnName == "series#")
                    {
                        seriesNumColumnIndex = i;
                    }
                    else if (columnName == "toy#")
                    {
                        toyColumnIndex = i;
                    }
                }

                var rows = table.QuerySelectorAll("tbody tr");
                foreach (var row in rows.Skip(1))
                {
                    var columns = row.Children;

                    HotWheelsModel hotWheelsModel = new HotWheelsModel()
                    {
                        ModelName = GetColumnValue(columns, carNameColumnIndex),
                        SeriesName = GetSeriesColumnValue(columns, seriesNameColumnIndex),
                        PhotoURL = ExtractPhotoUrl(columns, photoUrlColumnIndex),
                        SeriesNum = GetColumnValue(columns, seriesNumColumnIndex),
                        ToyNum = GetColumnValue(columns, toyColumnIndex),
                        YearProducedNum = GetColumnValue(columns, collectionNumColumnIndex),
                        YearProduced = year.ToString()
                    };

                    hotWheelsModels.Add(hotWheelsModel);
                }
            }

            return hotWheelsModels;
        }

        private string GetColumnValue(IHtmlCollection<IElement> columns, int columnIndex)
        {
            string output = string.Empty;
            if (columnIndex > -1)
            {
                IElement col = columns[columnIndex];
                if (col != null)
                {
                    output = columns[columnIndex].TextContent.Trim();
                }
            }

            return output;
        }

        private string GetSeriesColumnValue(IHtmlCollection<IElement> columns, int seriesNameColumnIndex)
        {
            string output = string.Empty;
            if (seriesNameColumnIndex != -1)
            {
                if (columns[seriesNameColumnIndex].QuerySelector(":nth-child(1)") != null)
                {
                    output = columns[seriesNameColumnIndex].QuerySelector(":nth-child(1)").TextContent.Trim();
                }
                else
                {
                    output = columns[seriesNameColumnIndex].TextContent.Trim();
                }
            }
            return output;
        }

        private string ExtractPhotoUrl(IHtmlCollection<IElement> columns, int photoColumnIndex)
        {
            string output = string.Empty;
            string anchorPhotoEl = columns[photoColumnIndex].OuterHtml;
            List<string> tempList = new List<string>(anchorPhotoEl.Split("href="));
            if (tempList.Count > 1)
            {
                List<string> photoUrlSegments = new List<string>(tempList[1].Split('"'));
                string photoUrl = photoUrlSegments[1].Trim();
                output = photoUrl;
            }

            return output;
        }
    }
}
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace WikiHotWheelsWebScraper.Services
{
    public class ScrapeHotWheelsWiki
    {
        private readonly IHtmlDocument _mainPage;
        private readonly HttpClient _httpClient = new HttpClient();

        public ScrapeHotWheelsWiki()
        {
            _mainPage = ScrapeMainPage().Result;
        }

        public async Task<IHtmlDocument> ScrapeMainPage()
        {
            
            HttpResponseMessage request = await _httpClient.GetAsync("https://hotwheels.fandom.com/wiki/Hot_Wheels");

            // Check if the response is successful
            Stream response = await request.Content.ReadAsStreamAsync();

            // Create a new HtmlParser object that will be used to parse the HTML content in a structured way
            HtmlParser parser = new HtmlParser();

            // IHtmlDocument is an interface that represents an HTML document with a DOM tree
            IHtmlDocument document = parser.ParseDocument(response);
            return document;
        }

        public async Task<IHtmlDocument> ScrapeYearCollection(int year)
        {
            HttpResponseMessage request = await _httpClient.GetAsync($"https://hotwheels.fandom.com/wiki/List_of_{year}_Hot_Wheels");

            Stream response = await request.Content.ReadAsStreamAsync();

            HtmlParser parser = new HtmlParser();

            IHtmlDocument yearPage = parser.ParseDocument(response);

            return yearPage;
        }

        public List<int> GetAllAvailableYears()
        {
            ScrapeHotWheelsWiki hwScraper = new ScrapeHotWheelsWiki();
            IHtmlDocument doc = _mainPage;

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
    }
}

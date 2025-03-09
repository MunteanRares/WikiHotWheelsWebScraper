using WikiHotWheelsWebScraper.Models;

namespace WikiHotWheelsWebScraper.Services
{
    public interface IScrapeHotWheelsWiki
    {
        Task<List<HotWheelsModel>> DefaultDataBasePopulation(int year);
        Task<List<int>> GetAllAvailableYears();
    }
}
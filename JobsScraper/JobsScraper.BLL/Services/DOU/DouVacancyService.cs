using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services.DOU
{
    public class DouVacancyService : IDouVacancyService
    {
        private readonly IDouHtmlLoader douHtmlLoader;
        private readonly IDouHtmlParser douHtmlParser;

        public DouVacancyService(IDouHtmlLoader douHtmlLoader, IDouHtmlParser douHtmlParser)
        {
            this.douHtmlParser = douHtmlParser;
            this.douHtmlLoader = douHtmlLoader;
        }

        public async Task<IEnumerable<Vacancy>> GetDouVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string? douHtml = await this.douHtmlLoader.LoadJobBoardHTMLAsync(jobSearchModel, token);
            var douVacancies = await this.douHtmlParser.ParseJobBoardHTMLAsync(douHtml, token);
            return douVacancies;
        }
    }
}

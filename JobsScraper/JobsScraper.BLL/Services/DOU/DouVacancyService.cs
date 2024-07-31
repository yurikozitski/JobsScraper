using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services.DOU
{
    public class DouVacancyService : IDouVacancyService
    {
        private readonly IDouRequestStringBuilder douRequestStringBuilder;
        private readonly IDouHtmlLoader douHtmlLoader;
        private readonly IDouHtmlParser douHtmlParser;

        public DouVacancyService(
            IDouRequestStringBuilder douRequestStringBuilder,
            IDouHtmlLoader douHtmlLoader,
            IDouHtmlParser douHtmlParser)
        {
            this.douRequestStringBuilder = douRequestStringBuilder;
            this.douHtmlParser = douHtmlParser;
            this.douHtmlLoader = douHtmlLoader;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string requestString = this.douRequestStringBuilder.GetRequestString(jobSearchModel);
            string? douHtml = await this.douHtmlLoader.LoadJobBoardHTMLAsync(requestString, token);
            var douVacancies = await this.douHtmlParser.ParseJobBoardHTMLAsync(douHtml, token);
            return douVacancies;
        }
    }
}

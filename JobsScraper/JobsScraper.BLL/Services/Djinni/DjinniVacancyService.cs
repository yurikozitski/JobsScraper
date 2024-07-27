using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniVacancyService : IDjinniVacancyService
    {
        private readonly IDjinniHtmlLoader djinniHtmlLoader;
        private readonly IDjinniHtmlParser djinniHtmlParser;

        public DjinniVacancyService(IDjinniHtmlLoader djinniHtmlLoader, IDjinniHtmlParser djinniHtmlParser)
        {
            this.djinniHtmlParser = djinniHtmlParser;
            this.djinniHtmlLoader = djinniHtmlLoader;
        }

        public async Task<IEnumerable<Vacancy>> GetDjinniVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string? djinniHtml = await this.djinniHtmlLoader.LoadJobBoardHTMLAsync(jobSearchModel, token);
            var djinniVacancies = await this.djinniHtmlParser.ParseJobBoardHTMLAsync(djinniHtml, token);
            return djinniVacancies;
        }
    }
}

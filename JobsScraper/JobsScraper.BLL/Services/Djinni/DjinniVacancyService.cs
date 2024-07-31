using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniVacancyService : IDjinniVacancyService
    {
        private readonly IDjinniRequestStringBuilder requestStringBuilder;
        private readonly IDjinniHtmlLoader djinniHtmlLoader;
        private readonly IDjinniHtmlParser djinniHtmlParser;

        public DjinniVacancyService(
            IDjinniRequestStringBuilder requestStringBuilder,
            IDjinniHtmlLoader djinniHtmlLoader,
            IDjinniHtmlParser djinniHtmlParser)
        {
            this.requestStringBuilder = requestStringBuilder;
            this.djinniHtmlParser = djinniHtmlParser;
            this.djinniHtmlLoader = djinniHtmlLoader;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string requestString = this.requestStringBuilder.GetRequestString(jobSearchModel);
            string? djinniHtml = await this.djinniHtmlLoader.LoadJobBoardHTMLAsync(requestString, token);
            var djinniVacancies = await this.djinniHtmlParser.ParseJobBoardHTMLAsync(djinniHtml, token);
            return djinniVacancies;
        }
    }
}

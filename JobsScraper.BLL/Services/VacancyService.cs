using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IDjinniHtmlLoader djinniHtmlLoader;
        private readonly IDouHtmlLoader douHtmlLoader;
        private readonly IDjinniHtmlParser djinniHtmlParser;
        private readonly IDouHtmlParser douHtmlParser;

        public VacancyService(
            IDjinniHtmlLoader djinniHtmlLoader,
            IDouHtmlLoader douHtmlLoader,
            IDjinniHtmlParser djinniHtmlParser,
            IDouHtmlParser douHtmlParser)
        {
            this.douHtmlLoader = douHtmlLoader;
            this.douHtmlParser = douHtmlParser;
            this.djinniHtmlLoader = djinniHtmlLoader;
            this.djinniHtmlParser = djinniHtmlParser;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(TimeSpan.FromMilliseconds(10_000));

            List<Vacancy> vacancies = new();

            var djinniHtmlLoadTask = this.djinniHtmlLoader.LoadJobBoardHTMLAsync(jobSearchModel, cts.Token);
            var douHtmlLoadTask = this.douHtmlLoader.LoadJobBoardHTMLAsync(jobSearchModel, cts.Token);

            await Task.WhenAll(djinniHtmlLoadTask, douHtmlLoadTask);

            var djinniHtmlPasreTask = this.djinniHtmlParser.ParseJobBoardHTMLAsync(djinniHtmlLoadTask.Result!, cts.Token);
            var douHtmlPasreTask = this.douHtmlParser.ParseJobBoardHTMLAsync(douHtmlLoadTask.Result!, cts.Token);

            await Task.WhenAll(djinniHtmlPasreTask, douHtmlPasreTask);

            vacancies.AddRange(djinniHtmlPasreTask.Result);
            vacancies.AddRange(douHtmlPasreTask.Result);

            return vacancies;
        }
    }
}

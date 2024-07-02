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

        public VacancyService(
            IDjinniHtmlLoader djinniHtmlLoader,
            IDouHtmlLoader douHtmlLoader,
            IDjinniHtmlParser djinniHtmlParser)
        {
            this.douHtmlLoader = douHtmlLoader;
            this.djinniHtmlLoader = djinniHtmlLoader;
            this.djinniHtmlParser = djinniHtmlParser;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            //cts.CancelAfter(TimeSpan.FromMilliseconds(10_000));

            List<Vacancy> vacancies = new();

            //var djinniHtmlLoadTask = this.djinniHtmlLoader.LoadJobBoardHTMLAsync(jobSearchModel, cts.Token);
            var douHtmlLoadTask = this.douHtmlLoader.LoadJobBoardHTMLAsync(jobSearchModel, cts.Token);

            await Task.WhenAll(/*djinniHtmlLoadTask, */douHtmlLoadTask);

            //var djinniHtmlPasreTask = this.djinniHtmlParser.ParseJobBoardHTMLAsync(djinniHtmlLoadTask.Result!, cts.Token);

            //await Task.WhenAll(djinniHtmlPasreTask);

            //vacancies.AddRange(djinniHtmlPasreTask.Result);

            return vacancies;
        }
    }
}

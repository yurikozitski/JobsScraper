using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IDouVacancyService douVacancyService;
        private readonly IDjinniVacancyService djinniVacancyService;

        public VacancyService(
            IDjinniVacancyService djinniVacancyService,
            IDouVacancyService douVacancyService)
        {
            this.djinniVacancyService = djinniVacancyService;
            this.douVacancyService = douVacancyService;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(TimeSpan.FromMilliseconds(10_000));

            List<Vacancy> vacancies = new();

            var djinniVacanciesTask = this.djinniVacancyService.GetDjinniVacanciesAsync(jobSearchModel, cts.Token);
            var douVacanciesTask = this.douVacancyService.GetDouVacanciesAsync(jobSearchModel, cts.Token);

            await Task.WhenAll(djinniVacanciesTask, douVacanciesTask);

            vacancies.AddRange(djinniVacanciesTask.Result);
            vacancies.AddRange(douVacanciesTask.Result);

            return vacancies;
        }
    }
}

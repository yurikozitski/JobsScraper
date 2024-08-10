using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Interfaces.Recruitika;
using JobsScraper.BLL.Interfaces.RobotaUa;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IDouVacancyService douVacancyService;
        private readonly IDjinniVacancyService djinniVacancyService;
        private readonly IRobotaUaVacancyService robotaUaVacancyService;
        private readonly IRecruitikaVacancyService recruitikaVacancyService;

        public VacancyService(
            IDjinniVacancyService djinniVacancyService,
            IDouVacancyService douVacancyService,
            IRobotaUaVacancyService robotaUaVacancyService,
            IRecruitikaVacancyService recruitikaVacancyService)
        {
            this.djinniVacancyService = djinniVacancyService;
            this.douVacancyService = douVacancyService;
            this.robotaUaVacancyService = robotaUaVacancyService;
            this.recruitikaVacancyService = recruitikaVacancyService;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(TimeSpan.FromMilliseconds(60_000));

            List<Vacancy> vacancies = new();

            var djinniVacanciesTask = this.djinniVacancyService.GetVacanciesAsync(jobSearchModel, cts.Token);
            var douVacanciesTask = this.douVacancyService.GetVacanciesAsync(jobSearchModel, cts.Token);
            var robotaUaVacanciesTask = this.robotaUaVacancyService.GetVacanciesAsync(jobSearchModel, cts.Token);
            var recruitikaVacanciesTask = this.recruitikaVacancyService.GetVacanciesAsync(jobSearchModel, cts.Token);

            await Task.WhenAll(
                djinniVacanciesTask,
                douVacanciesTask,
                robotaUaVacanciesTask,
                recruitikaVacanciesTask);

            vacancies.AddRange(djinniVacanciesTask.Result);
            vacancies.AddRange(douVacanciesTask.Result);
            vacancies.AddRange(robotaUaVacanciesTask.Result);
            vacancies.AddRange(recruitikaVacanciesTask.Result);

            return vacancies;
        }
    }
}

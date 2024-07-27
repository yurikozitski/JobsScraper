using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Interfaces.Djinni
{
    public interface IDjinniVacancyService
    {
        Task<IEnumerable<Vacancy>> GetDjinniVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token);
    }
}

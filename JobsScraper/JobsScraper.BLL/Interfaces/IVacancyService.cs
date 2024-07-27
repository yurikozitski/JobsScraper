using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Interfaces
{
    public interface IVacancyService
    {
        Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token);
    }
}

using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Interfaces.DOU
{
    public interface IDouVacancyService
    {
        Task<IEnumerable<Vacancy>> GetDouVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token);
    }
}

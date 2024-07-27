using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Interfaces
{
    public interface IHtmlLoader
    {
        Task<string?> LoadJobBoardHTMLAsync(JobSearchModel jobSearchModel, CancellationToken token);
    }
}

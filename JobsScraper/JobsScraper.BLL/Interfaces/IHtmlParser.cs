using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Interfaces
{
    public interface IHtmlParser
    {
        Task<IEnumerable<Vacancy>> ParseJobBoardHTMLAsync(string? jobBoardHTML, CancellationToken token);
    }
}

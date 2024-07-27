using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Interfaces
{
    public interface IRequestStringBuilder
    {
        string GetRequestString(JobSearchModel jobSearchModel);
    }
}

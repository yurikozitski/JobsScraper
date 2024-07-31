using JobsScraper.BLL.Interfaces.RobotaUa;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services.RobotaUa
{
    public class RobotaUaVacancyService : IRobotaUaVacancyService
    {
        private readonly IRobotaUaRequestStringBuilder robotaUaRequestStringBuilder;
        private readonly IRobotaUaHtmlLoader robotaUaHtmlLoader;
        private readonly IRobotaUaHtmlParser robotaUaHtmlParser;

        public RobotaUaVacancyService(
            IRobotaUaRequestStringBuilder robotaUaRequestStringBuilder,
            IRobotaUaHtmlLoader robotaUaHtmlLoader,
            IRobotaUaHtmlParser robotaUaHtmlParser)
        {
            this.robotaUaRequestStringBuilder = robotaUaRequestStringBuilder;
            this.robotaUaHtmlParser = robotaUaHtmlParser;
            this.robotaUaHtmlLoader = robotaUaHtmlLoader;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string requestString = this.robotaUaRequestStringBuilder.GetRequestString(jobSearchModel);
            string? robotaUaHtml = await this.robotaUaHtmlLoader.LoadJobBoardHTMLAsync(requestString, token);
            var robotaUaVacancies = await this.robotaUaHtmlParser.ParseJobBoardHTMLAsync(robotaUaHtml, token);
            return robotaUaVacancies;
        }
    }
}

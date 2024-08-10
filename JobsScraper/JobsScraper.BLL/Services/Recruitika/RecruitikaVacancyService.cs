using JobsScraper.BLL.Interfaces.Recruitika;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Services.Recruitika
{
    public class RecruitikaVacancyService : IRecruitikaVacancyService
    {
        private readonly IRecruitikaRequestStringBuilder requestStringBuilder;
        private readonly IRecruitikaHtmlLoader recruitikaHtmlLoader;
        private readonly IRecruitikaHtmlParser recruitikaHtmlParser;

        public RecruitikaVacancyService(
            IRecruitikaRequestStringBuilder requestStringBuilder,
            IRecruitikaHtmlLoader recruitikaHtmlLoader,
            IRecruitikaHtmlParser recruitikaHtmlParser)
        {
            this.requestStringBuilder = requestStringBuilder;
            this.recruitikaHtmlParser = recruitikaHtmlParser;
            this.recruitikaHtmlLoader = recruitikaHtmlLoader;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string requestString = this.requestStringBuilder.GetRequestString(jobSearchModel);
            string? recruitikaHtml = await this.recruitikaHtmlLoader.LoadJobBoardHTMLAsync(requestString, token);
            var recruitikaVacancies = await this.recruitikaHtmlParser.ParseJobBoardHTMLAsync(recruitikaHtml, token);
            return recruitikaVacancies;
        }
    }
}

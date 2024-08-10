using JobsScraper.BLL.Interfaces.Recruitika;
using Microsoft.Extensions.Logging;

namespace JobsScraper.BLL.Services.Recruitika
{
    public class RecruitikaHtmlLoader : IRecruitikaHtmlLoader
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<RecruitikaHtmlLoader> logger;

        public RecruitikaHtmlLoader(
            IHttpClientFactory httpClientFactory,
            ILogger<RecruitikaHtmlLoader> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<string?> LoadJobBoardHTMLAsync(string requestString, CancellationToken token)
        {
            var httpClient = this.httpClientFactory.CreateClient();

            string? recruitikaHtml = null;

            try
            {
                recruitikaHtml = await httpClient.GetStringAsync(requestString, token);
            }
            catch (HttpRequestException ex)
            {
                this.logger.LogError(ex, "Unable to load page from djiini");
            }

            return recruitikaHtml;
        }
    }
}

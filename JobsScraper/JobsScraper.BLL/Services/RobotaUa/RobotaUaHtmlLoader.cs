using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Interfaces.RobotaUa;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Services.Djinni;
using Microsoft.Extensions.Logging;

namespace JobsScraper.BLL.Services.RobotaUa
{
    public class RobotaUaHtmlLoader : IRobotaUaHtmlLoader
    {
        private readonly IRobotaUaRequestStringBuilder robotaUaRequestStringBuilder;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<RobotaUaHtmlLoader> logger;

        public RobotaUaHtmlLoader(
            IRobotaUaRequestStringBuilder robotaUaRequestStringBuilder,
            IHttpClientFactory httpClientFactory,
            ILogger<RobotaUaHtmlLoader> logger)
        {
            this.robotaUaRequestStringBuilder = robotaUaRequestStringBuilder;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<string?> LoadJobBoardHTMLAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string requestString = this.robotaUaRequestStringBuilder.GetRequestString(jobSearchModel);

            var httpClient = this.httpClientFactory.CreateClient();

            string? djinniHtml = null;

            try
            {
                djinniHtml = await httpClient.GetStringAsync(requestString, token);
            }
            catch (HttpRequestException ex)
            {
                this.logger.LogError(ex, "Unable to load page from robota.ua");
            }

            return djinniHtml;
        }
    }
}

using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Logging;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniHtmlLoader : IDjinniHtmlLoader
    {
        private readonly IDjinniRequestStringBuilder djinniRequestStringBuilder;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<DjinniHtmlLoader> logger;

        public DjinniHtmlLoader(
            IDjinniRequestStringBuilder djinniRequestStringBuilder,
            IHttpClientFactory httpClientFactory,
            ILogger<DjinniHtmlLoader> logger)
        {
            this.djinniRequestStringBuilder = djinniRequestStringBuilder;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<string?> LoadJobBoardHTMLAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            string requestString = this.djinniRequestStringBuilder.GetRequestString(jobSearchModel);

            var httpClient = this.httpClientFactory.CreateClient();

            string? djinniHtml = null;

            try
            {
                djinniHtml = await httpClient.GetStringAsync(requestString, token);
            }
            catch (HttpRequestException ex)
            {
                this.logger.LogError(ex, "Unable to load page from djiini");
            }

            return djinniHtml;
        }
    }
}

using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Logging;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniHtmlLoader : IDjinniHtmlLoader
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<DjinniHtmlLoader> logger;

        public DjinniHtmlLoader(
            IHttpClientFactory httpClientFactory,
            ILogger<DjinniHtmlLoader> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<string?> LoadJobBoardHTMLAsync(string requestString, CancellationToken token)
        {
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

using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniHtmlLoader : IDjinniHtmlLoader
    {
        private readonly IDjinniRequestStringBuilder djinniRequestStringBuilder;
        private readonly IHttpClientFactory httpClientFactory;

        public DjinniHtmlLoader(IDjinniRequestStringBuilder djinniRequestStringBuilder, IHttpClientFactory httpClientFactory)
        {
            this.djinniRequestStringBuilder = djinniRequestStringBuilder;
            this.httpClientFactory = httpClientFactory;
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
                //TODO Logging here
                Console.WriteLine(ex.Message);
            }

            return djinniHtml;
        }
    }
}

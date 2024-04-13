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

        public Task<string> LoadJobBoardHTML(JobSearchModel jobSearchModel)
        {
            string requestString = djinniRequestStringBuilder.GetRequestString(jobSearchModel);

            var httpClient = httpClientFactory.CreateClient();
            return httpClient.GetStringAsync(requestString);
        }
    }
}

using HtmlAgilityPack;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniHtmlParser : IDjinniHtmlParser
    {
        private readonly IDjinniRequestStringBuilder djinniRequestStringBuilder;
        private readonly IHttpClientFactory httpClientFactory;

        public DjinniHtmlParser(IDjinniRequestStringBuilder djinniRequestStringBuilder, IHttpClientFactory httpClientFactory)
        {
            this.djinniRequestStringBuilder = djinniRequestStringBuilder;
            this.httpClientFactory = httpClientFactory;
        }

        public Task<IEnumerable<Vacancy>> ParseJobBoardHTML(string jobBoardHTML)
        {
            return Task.Run(async () => {
                var doc = new HtmlDocument();
                doc.LoadHtml(jobBoardHTML);

                var vacancyNodes = doc.DocumentNode.SelectNodes("//li[@class = 'list-jobs__item job-list__item']");

                if(vacancyNodes == null)
                {
                    return Enumerable.Empty<Vacancy>();
                }

                var vacancies = GetVacancyList(vacancyNodes);

                int? numberOfAdditionalPages = GetNumberOfPages(doc.DocumentNode);

                if(numberOfAdditionalPages != null)
                {
                    var additionalPages = await LoadAdditionalPages(
                        httpClientFactory.CreateClient(), 
                        djinniRequestStringBuilder.RequestString, 
                        (int)numberOfAdditionalPages);

                    foreach (string page in additionalPages)
                    {
                        var pageDoc = new HtmlDocument();
                        pageDoc.LoadHtml(page);

                        vacancies.AddRange(GetVacancyList(pageDoc.DocumentNode.SelectNodes("//li[@class = 'list-jobs__item job-list__item']")));
                    }
                }

                return vacancies;
            });
        }

        private static List<Vacancy> GetVacancyList(HtmlNodeCollection vacancyNodes)
        {
            List<Vacancy> vacancies = new();

            foreach (var vacancyNode in vacancyNodes)
            {
                Vacancy vacancy = new Vacancy()
                {
                    //TODO finish this job
                    WebSite = "Djinni",

                    JobTitle = vacancyNode.SelectSingleNode(".//a[@class = 'h3 job-list-item__link']").InnerText
                        .Replace("\n", String.Empty)
                        .Trim(),

                    Company = vacancyNode.SelectSingleNode(".//a[@class = 'mr-2']").InnerText
                        .Replace("\n", String.Empty)
                        .Replace(" ", String.Empty)
                        .Trim(),

                    Salary = vacancyNode.SelectSingleNode(".//span[@class = 'public-salary-item']")?.InnerText
                        .Replace("\n", String.Empty)
                        .Replace(" ", String.Empty)
                        .Trim(),

                    Location = vacancyNode.SelectSingleNode(".//span[@class = 'location-text']")?.InnerText
                        .Replace("\n", String.Empty)
                        .Replace(" ", String.Empty)
                        .Trim(),

                    Description = vacancyNode.SelectSingleNode(".//span[contains(@id, 'job-description')]").Attributes["data-truncated-text"].Value,
                };
                vacancies.Add(vacancy);
            }

            return vacancies;
        }

        private static int? GetNumberOfPages(HtmlNode document)
        {
            int? numberOfPages = null;
            var paginationNode = document.SelectSingleNode("//ul[@class = 'pagination pagination_with_numbers']");

            if (paginationNode != null &&
                paginationNode.HasChildNodes && 
                paginationNode.ChildNodes.Count >= 2 )
            {
                var pagesString = paginationNode.ChildNodes[paginationNode.ChildNodes.Count - 2].ChildNodes[0].InnerText.Trim();
                int.TryParse(pagesString, out int result);
                numberOfPages = result;
            }

            return numberOfPages;
        }

        private static async Task<IEnumerable<string>> LoadAdditionalPages(HttpClient httpClient, string requstPath, int numberOfPages)
        {
            List<Task<string>> pagesTasks = new ();

            for (int page = 2; page <= numberOfPages; page++)
            {
                var pageTask = httpClient.GetStringAsync(requstPath + $"page={page}");
                pagesTasks.Add(pageTask);
            }

            var pages = await Task.WhenAll(pagesTasks);
            return pages.ToList();
        }
    }
}

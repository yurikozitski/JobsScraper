using HtmlAgilityPack;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JobsScraper.BLL.Enums;
using System.Globalization;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniHtmlParser : IDjinniHtmlParser
    {
        private const string webSite = "Djinni";
        private const string shortDomain = "https://djinni.co";

        private readonly IDjinniRequestStringBuilder djinniRequestStringBuilder;
        private readonly IHttpClientFactory httpClientFactory;

        public DjinniHtmlParser(IDjinniRequestStringBuilder djinniRequestStringBuilder, IHttpClientFactory httpClientFactory)
        {
            this.djinniRequestStringBuilder = djinniRequestStringBuilder;
            this.httpClientFactory = httpClientFactory;
        }

        public Task<IEnumerable<Vacancy>> ParseJobBoardHTMLAsync(string jobBoardHTML)
        {
            return Task.Run(async () => {

                if(jobBoardHTML == null)
                {
                    return Enumerable.Empty<Vacancy>();
                }

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
                    var additionalPages = await LoadAdditionalPagesAsync(
                        this.httpClientFactory.CreateClient(), 
                        this.djinniRequestStringBuilder.RequestString, 
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
                string? localLink = vacancyNode.SelectSingleNode(".//a[@class = 'h3 job-list-item__link']")?.Attributes["href"]?.Value
                        .Replace("\n", String.Empty)
                        .Trim()!;

                if (localLink == null)
                {
                    var message = $"Can't parse local link from {nameof(JobBoards.Djinni)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string link = shortDomain + localLink;

                string jobTitle = vacancyNode.SelectSingleNode(".//a[@class = 'h3 job-list-item__link']")?.InnerText
                        .Replace("\n", String.Empty)
                        .Trim()!;

                if (jobTitle == null)
                {
                    var message = $"Can't parse job title from {nameof(JobBoards.Djinni)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string company = vacancyNode.SelectSingleNode(".//a[@class = 'mr-2']")?.InnerText
                        .Replace("\n", String.Empty)
                        .Trim()!;

                if (company == null)
                {
                    var message = $"Can't parse company name from {nameof(JobBoards.Djinni)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string? publicationDateString = vacancyNode.SelectSingleNode(".//span[@class = 'mr-2 nobr']")?.Attributes["title"]?.Value.Trim();
                DateOnly.TryParse(publicationDateString?.Substring(6), CultureInfo.GetCultureInfo("ru-RU"), out DateOnly publicationDate);

                Vacancy vacancy = new Vacancy()
                {
                    WebSite = webSite,
                    Link = link,
                    JobTitle = jobTitle,
                    Company = company,

                    Salary = vacancyNode.SelectSingleNode(".//span[@class = 'public-salary-item']")?.InnerText
                        .Replace("\n", String.Empty)
                        .Trim(),

                    JobType = GetJobType(vacancyNode),

                    Location = vacancyNode.SelectSingleNode(".//span[@class = 'location-text']")?.InnerText
                        .Replace("\n", String.Empty)
                        .Replace(" ", String.Empty)
                        .Trim(),

                    Description = vacancyNode.SelectSingleNode(".//span[contains(@id, 'job-description')]")?.Attributes["data-truncated-text"]?.Value
                        .Replace("\n", String.Empty)
                        .Replace("\r", String.Empty)
                        .Replace("\t", String.Empty)
                        .Replace("&#39;", "'")
                        .Trim(),

                    PublicationDate = publicationDate,
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
                var pagesString = paginationNode.ChildNodes[paginationNode.ChildNodes.Count - 4].ChildNodes[1].InnerText.Trim();
                int.TryParse(pagesString, out int result);
                numberOfPages = result;
            }

            return numberOfPages;
        }

        private static async Task<IEnumerable<string>> LoadAdditionalPagesAsync(HttpClient httpClient, string requstPath, int numberOfPages)
        {
            List<Task<string>> pagesTasks = new ();

            for (int page = 2; page <= numberOfPages; page++)
            {
                var pageTask = httpClient.GetStringAsync(requstPath + $"&page={page}");
                pagesTasks.Add(pageTask);
            }

            string[] pages;

            try
            {
                pages = await Task.WhenAll(pagesTasks);
            }
            catch (HttpRequestException ex)
            {
                //TODO Log exeption
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<string>();
            }

            return pages.ToList();
        }

        private static string? GetJobType(HtmlNode vacancyNode) 
        {
            string? jobType = null;

            if(vacancyNode != null)
            {
                var vacancyInfoNodes = vacancyNode.SelectSingleNode(".//div[@class = 'job-list-item__job-info font-weight-500']")?.ChildNodes
                    .Where(node => node.HasClass("nobr"))
                    .Select(node => node.ChildNodes[1]);

                if(vacancyInfoNodes != null)
                {
                    foreach (var node in vacancyInfoNodes)
                    {
                        string? innerText = node?.InnerText?.Trim();

                        if (innerText == "Тільки віддалено" ||
                            innerText == "Office або Remote" ||
                            innerText == "Гібридна робота" ||
                            innerText == "Тільки офіс")
                        {
                            jobType = innerText;
                        }
                    }
                }
            }

            return jobType;
        }
    }
}

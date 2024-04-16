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
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniHtmlParser : IDjinniHtmlParser
    {
        private const string webSite = "Djinni";

        private readonly IDjinniRequestStringBuilder djinniRequestStringBuilder;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public DjinniHtmlParser(IDjinniRequestStringBuilder djinniRequestStringBuilder, 
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            this.djinniRequestStringBuilder = djinniRequestStringBuilder;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public Task<IEnumerable<Vacancy>> ParseJobBoardHTMLAsync(string jobBoardHTML, CancellationToken token)
        {
            return Task.Run(async () => {

                if(jobBoardHTML == null)
                {
                    return Enumerable.Empty<Vacancy>();
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(jobBoardHTML);

                var vacancyNodes = doc.DocumentNode.SelectNodes(this.configuration["Djinni:XPaths:VacancyList"]);

                if(vacancyNodes == null)
                {
                    return Enumerable.Empty<Vacancy>();
                }

                var vacancies = GetVacancyList(vacancyNodes, this.configuration, token);

                int? numberOfAdditionalPages = GetNumberOfPages(doc.DocumentNode, this.configuration);

                if(numberOfAdditionalPages != null)
                {
                    var additionalPages = await LoadAdditionalPagesAsync(
                        this.httpClientFactory.CreateClient(), 
                        this.djinniRequestStringBuilder.RequestString, 
                        (int)numberOfAdditionalPages,
                        token);

                    foreach (string page in additionalPages)
                    {
                        var pageDoc = new HtmlDocument();
                        pageDoc.LoadHtml(page);

                        vacancies.AddRange(GetVacancyList(pageDoc.DocumentNode.SelectNodes("//li[@class = 'list-jobs__item job-list__item']"), 
                            this.configuration, token));
                    }
                }

                return vacancies;
            });
        }

        private static List<Vacancy> GetVacancyList(HtmlNodeCollection vacancyNodes, IConfiguration configuration, CancellationToken token)
        {
            List<Vacancy> vacancies = new();

            foreach (var vacancyNode in vacancyNodes)
            {
                token.ThrowIfCancellationRequested();

                string? localLink = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:LocalLink"])?.Attributes["href"]?.Value
                        .Replace("\n", String.Empty)
                        .Trim()!;

                if (localLink == null)
                {
                    var message = $"Can't parse local link from {nameof(JobBoards.Djinni)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string link = configuration["Djinni:ShortDomain"] + localLink;

                string jobTitle = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:JobTitle"])?.InnerText
                        .Replace("\n", String.Empty)
                        .Trim()!;

                if (jobTitle == null)
                {
                    var message = $"Can't parse job title from {nameof(JobBoards.Djinni)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string company = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Company"])?.InnerText
                        .Replace("\n", String.Empty)
                        .Trim()!;

                if (company == null)
                {
                    var message = $"Can't parse company name from {nameof(JobBoards.Djinni)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string? publicationDateString = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:PublicationDate"])?.Attributes["title"]?.Value.Trim();
                DateOnly.TryParse(publicationDateString?.Substring(6), CultureInfo.GetCultureInfo("ru-RU"), out DateOnly publicationDate);

                Vacancy vacancy = new Vacancy()
                {
                    WebSite = webSite,
                    Link = link,
                    JobTitle = jobTitle,
                    Company = company,

                    Salary = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Salary"])?.InnerText
                        .Replace("\n", String.Empty)
                        .Trim(),

                    JobType = GetJobType(vacancyNode, configuration),

                    Location = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Location"])?.InnerText
                        .Replace("\n", String.Empty)
                        .Replace(" ", String.Empty)
                        .Trim(),

                    Description = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Description"])?.Attributes["data-truncated-text"]?.Value
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

        private static int? GetNumberOfPages(HtmlNode document, IConfiguration configuration)
        {
            int? numberOfPages = null;
            var paginationNode = document.SelectSingleNode(configuration["Djinni:XPaths:Pagination"]);

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

        private static async Task<IEnumerable<string>> LoadAdditionalPagesAsync(HttpClient httpClient, string requstPath, int numberOfPages, CancellationToken token)
        {
            List<Task<string>> pagesTasks = new ();

            for (int page = 2; page <= numberOfPages; page++)
            {
                var pageTask = httpClient.GetStringAsync(requstPath + $"&page={page}", token);
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

        private static string? GetJobType(HtmlNode vacancyNode, IConfiguration configuration) 
        {
            string? jobType = null;

            if(vacancyNode != null)
            {
                var vacancyInfoNodes = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:JobType"])?.ChildNodes
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

using System.Globalization;
using HtmlAgilityPack;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniHtmlParser : IDjinniHtmlParser
    {
        private const string WebSiteName = "Djinni";

        private readonly IDjinniRequestStringBuilder djinniRequestStringBuilder;
        private readonly IDjinniHtmlLoader djinniHtmlLoader;
        private readonly IConfiguration configuration;
        private readonly ILogger<DjinniHtmlParser> logger;

        public DjinniHtmlParser(
            IDjinniRequestStringBuilder djinniRequestStringBuilder,
            IDjinniHtmlLoader djinniHtmlLoader,
            IConfiguration configuration,
            ILogger<DjinniHtmlParser> logger)
        {
            this.djinniRequestStringBuilder = djinniRequestStringBuilder;
            this.djinniHtmlLoader = djinniHtmlLoader;
            this.configuration = configuration;
            this.logger = logger;
        }

        public Task<IEnumerable<Vacancy>> ParseJobBoardHTMLAsync(string? jobBoardHTML, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(jobBoardHTML))
                {
                    this.logger.LogError($"Attempt to parse null or empty html string from {nameof(JobBoards.Djinni)}");
                    return Enumerable.Empty<Vacancy>();
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(jobBoardHTML);

                var vacancyNodes = doc.DocumentNode.SelectNodes(this.configuration["Djinni:XPaths:VacancyList"]);

                if (vacancyNodes == null)
                {
                    string message = $"Can't get vacancy nodes from {nameof(JobBoards.Djinni)}, XPath: {this.configuration["Djinni:XPaths:VacancyList"]}";
                    this.logger.LogError(message);
                    return Enumerable.Empty<Vacancy>();
                }

                var vacancies = this.GetVacancyList(vacancyNodes, this.configuration, token);

                int? numberOfAdditionalPages = GetNumberOfPages(doc.DocumentNode, this.configuration);

                if (numberOfAdditionalPages != null)
                {
                    var additionalPages = await this.LoadAdditionalPagesAsync(
                        this.djinniHtmlLoader,
                        this.djinniRequestStringBuilder.RequestString,
                        (int)numberOfAdditionalPages,
                        token);

                    foreach (string page in additionalPages)
                    {
                        var pageDoc = new HtmlDocument();
                        pageDoc.LoadHtml(page);

                        vacancies.AddRange(this.GetVacancyList(
                            pageDoc.DocumentNode.SelectNodes("//li[@class = 'list-jobs__item job-list__item']"),
                            this.configuration,
                            token));
                    }
                }

                return vacancies;
            });
        }

        private List<Vacancy> GetVacancyList(HtmlNodeCollection vacancyNodes, IConfiguration configuration, CancellationToken token)
        {
            List<Vacancy> vacancies = new();

            foreach (var vacancyNode in vacancyNodes)
            {
                token.ThrowIfCancellationRequested();

                string? localLink = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:LocalLink"])?.Attributes["href"]?.Value
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (localLink == null)
                {
                    var message = $"Can't parse local link from {nameof(JobBoards.Djinni)}, XPath is {configuration["Djinni:XPaths:LocalLink"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string link = configuration["Djinni:ShortDomain"] + localLink;

                string jobTitle = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:JobTitle"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (jobTitle == null)
                {
                    var message = $"Can't parse job title from {nameof(JobBoards.Djinni)}, XPath is {configuration["Djinni:XPaths:JobTitle"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string company = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Company"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (company == null)
                {
                    var message = $"Can't parse company name from {nameof(JobBoards.Djinni)}, XPath is {configuration["Djinni:XPaths:Company"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string? publicationDateString = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:PublicationDate"])?.Attributes["title"]?.Value.Trim();
                DateOnly.TryParse(publicationDateString?.Substring(6), CultureInfo.GetCultureInfo("ru-RU"), out DateOnly publicationDate);

                Vacancy vacancy = new Vacancy()
                {
                    WebSite = WebSiteName,
                    Link = link,
                    JobTitle = jobTitle,
                    Company = company,

                    Salary = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Salary"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim(),

                    JobType = GetJobType(vacancyNode, configuration),

                    Location = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Location"])?.InnerText
                        .Replace("\n", string.Empty)
                        //.Replace(" ", string.Empty)
                        .Trim(),

                    Description = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:Description"])?.ChildNodes[1]?.InnerText
                        .Replace("\n", string.Empty)
                        .Replace("\r", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace("&#39;", "'")
                        .Trim(),

                    PublicationDate = publicationDate,
                };
                vacancies.Add(vacancy);
            }

            return vacancies;
        }

#pragma warning disable SA1204 // StaticElementsMustAppearBeforeInstanceElements
        private static int? GetNumberOfPages(HtmlNode document, IConfiguration configuration)
        {
            int? numberOfPages = null;
            var paginationNode = document.SelectSingleNode(configuration["Djinni:XPaths:Pagination"]);

            if (paginationNode != null &&
                paginationNode.HasChildNodes &&
                paginationNode.ChildNodes.Count >= 2)
            {
                var pagesString = paginationNode.ChildNodes[paginationNode.ChildNodes.Count - 4].ChildNodes[1].InnerText.Trim();
                int.TryParse(pagesString, out int result);
                numberOfPages = result;
            }

            return numberOfPages;
        }

        private async Task<IEnumerable<string>> LoadAdditionalPagesAsync(IDjinniHtmlLoader djinniHtmlLoader, string requstPath, int numberOfPages, CancellationToken token)
        {
            List<Task<string>> pagesTasks = new();

            for (int page = 2; page <= numberOfPages; page++)
            {
                var pageTask = djinniHtmlLoader.LoadJobBoardHTMLAsync(requstPath + $"&page={page}", token);
                pagesTasks.Add(pageTask!);
            }

            string[] pages;

            try
            {
                pages = await Task.WhenAll(pagesTasks);
            }
            catch (HttpRequestException ex)
            {
                this.logger.LogError(ex, $"Can't load additional pages for {nameof(JobBoards.Djinni)}");
                return Enumerable.Empty<string>();
            }

            return pages.ToList();
        }

        private static string? GetJobType(HtmlNode vacancyNode, IConfiguration configuration)
        {
            string? jobType = null;

            if (vacancyNode != null)
            {
                var vacancyInfoNodes = vacancyNode.SelectSingleNode(configuration["Djinni:XPaths:JobType"])?.ChildNodes
                    .Where(node => node.HasClass("nobr"))
                    .Select(node => node.ChildNodes[1]);

                if (vacancyInfoNodes != null)
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

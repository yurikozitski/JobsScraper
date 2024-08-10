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

                var vacancies = this.GetVacancyList(vacancyNodes, token);

                int? numberOfAdditionalPages = this.GetNumberOfPages(doc.DocumentNode);

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
                            pageDoc.DocumentNode.SelectNodes(this.configuration["Djinni:XPaths:VacancyList"]),
                            token));
                    }
                }

                return vacancies;
            });
        }

        private List<Vacancy> GetVacancyList(HtmlNodeCollection vacancyNodes, CancellationToken token)
        {
            List<Vacancy> vacancies = new();

            foreach (var vacancyNode in vacancyNodes)
            {
                token.ThrowIfCancellationRequested();

                string? localLink = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:LocalLink"])?.Attributes["href"]?.Value
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (localLink == null)
                {
                    var message = $"Can't parse local link from {nameof(JobBoards.Djinni)}, XPath is {this.configuration["Djinni:XPaths:LocalLink"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string link = this.configuration["Djinni:ShortDomain"] + localLink;

                string jobTitle = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:JobTitle"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (jobTitle == null)
                {
                    var message = $"Can't parse job title from {nameof(JobBoards.Djinni)}, XPath is {this.configuration["Djinni:XPaths:JobTitle"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string company = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:Company"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (company == null)
                {
                    var message = $"Can't parse company name from {nameof(JobBoards.Djinni)}, XPath is {this.configuration["Djinni:XPaths:Company"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string? publicationDateString = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:PublicationDate"])?.Attributes["data-original-title"]?.Value.Trim();
                DateOnly.TryParse(publicationDateString?.Substring(6), CultureInfo.GetCultureInfo("ru-RU"), out DateOnly publicationDate);

                Vacancy vacancy = new Vacancy()
                {
                    WebSite = WebSiteName,
                    Link = link,
                    JobTitle = jobTitle,
                    Company = company,

                    Salary = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:Salary"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim(),

                    JobType = this.GetJobType(vacancyNode),

                    Location = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:Location"])?.InnerText
                        .Replace("\n", string.Empty)
                        //.Replace(" ", string.Empty)
                        .Trim(),

                    Description = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:Description"])?.ChildNodes[1]?.InnerText
                        .Replace("\n", string.Empty)
                        .Replace("\r", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace("&#39;", "'")
                        .Trim(),

                    PublicationDate = publicationDate != default(DateOnly) ? publicationDate : null,
                };
                vacancies.Add(vacancy);
            }

            return vacancies;
        }

        private int? GetNumberOfPages(HtmlNode document)
        {
            int? numberOfPages = null;
            var paginationNode = document.SelectSingleNode(this.configuration["Djinni:XPaths:Pagination"]);

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

        private string? GetJobType(HtmlNode vacancyNode)
        {
            string? jobType = null;

            if (vacancyNode != null)
            {
                var remoteNode = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:JobType"])?.ChildNodes
                    .FirstOrDefault(node => node.InnerText.Trim() == "Тільки віддалено");
                if (remoteNode != null)
                    return "Тільки віддалено";

                var officeNode = vacancyNode.SelectSingleNode(this.configuration["Djinni:XPaths:JobType"])?.ChildNodes
                    .FirstOrDefault(node => node.InnerText.Trim() == "Тільки офіс");
                if (officeNode != null)
                    return "Тільки офіс";
            }

            return jobType;
        }
    }
}

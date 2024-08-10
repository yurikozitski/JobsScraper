using System.Globalization;
using HtmlAgilityPack;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Interfaces.Recruitika;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JobsScraper.BLL.Services.Recruitika
{
    public class RecruitikaHtmlParser : IRecruitikaHtmlParser
    {
        private const string WebSiteName = "Recruitika";

        private readonly IRecruitikaRequestStringBuilder recruitikaRequestStringBuilder;
        private readonly IRecruitikaHtmlLoader recruitikaHtmlLoader;
        private readonly IConfiguration configuration;
        private readonly ILogger<RecruitikaHtmlParser> logger;

        public RecruitikaHtmlParser(
            IRecruitikaRequestStringBuilder recruitikaRequestStringBuilder,
            IRecruitikaHtmlLoader recruitikaHtmlLoader,
            IConfiguration configuration,
            ILogger<RecruitikaHtmlParser> logger)
        {
            this.recruitikaRequestStringBuilder = recruitikaRequestStringBuilder;
            this.recruitikaHtmlLoader = recruitikaHtmlLoader;
            this.configuration = configuration;
            this.logger = logger;
        }

        public Task<IEnumerable<Vacancy>> ParseJobBoardHTMLAsync(string? jobBoardHTML, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(jobBoardHTML))
                {
                    this.logger.LogError($"Attempt to parse null or empty html string from {nameof(JobBoards.Recruitika)}");
                    return Enumerable.Empty<Vacancy>();
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(jobBoardHTML);

                var vacancyNodes = doc.DocumentNode.SelectNodes(this.configuration["Recruitika:XPaths:VacancyList"]);

                if (vacancyNodes == null)
                {
                    string message = $"Can't get vacancy nodes from {nameof(JobBoards.Recruitika)}, XPath: {this.configuration["Recruitika:XPaths:VacancyList"]}";
                    this.logger.LogError(message);
                    return Enumerable.Empty<Vacancy>();
                }

                var vacancies = this.GetVacancyList(vacancyNodes, token);

                int? numberOfPages = this.GetNumberOfPages(doc.DocumentNode);

                if (numberOfPages != null)
                {
                    var additionalPages = await this.LoadAdditionalPagesAsync(
                        this.recruitikaHtmlLoader,
                        this.recruitikaRequestStringBuilder.RequestString,
                        (int)numberOfPages,
                        token);

                    foreach (string page in additionalPages)
                    {
                        var pageDoc = new HtmlDocument();
                        pageDoc.LoadHtml(page);

                        vacancies.AddRange(this.GetVacancyList(
                            pageDoc.DocumentNode.SelectNodes(this.configuration["Recruitika:XPaths:VacancyList"]),
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

                string? localLink = vacancyNode.SelectSingleNode(this.configuration["Recruitika:XPaths:JobTitle"])?.FirstChild?.Attributes["href"]?.Value
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (localLink == null)
                {
                    var message = $"Can't parse local link from {nameof(JobBoards.Recruitika)}, XPath is {this.configuration["Recruitika:XPaths:JobTitle"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string link = this.configuration["Recruitika:ShortDomain"] + localLink;

                string jobTitle = vacancyNode.SelectSingleNode(this.configuration["Recruitika:XPaths:JobTitle"])?.FirstChild?.InnerText
                        .Replace("\n", " ")
                        .Trim()!;

                if (jobTitle == null)
                {
                    var message = $"Can't parse job title from {nameof(JobBoards.Recruitika)}, XPath is {this.configuration["Recruitika:XPaths:JobTitle"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string company = vacancyNode.SelectSingleNode(this.configuration["Recruitika:XPaths:Company"])?.InnerText
                        .Replace("\n", " ")
                        .Trim()!;

                if (company == null)
                {
                    var message = $"Can't parse company name from {nameof(JobBoards.Recruitika)}, XPath is {this.configuration["Recruitika:XPaths:Company"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string? publicationDateString = vacancyNode.SelectSingleNode(this.configuration["Recruitika:XPaths:PublicationDate"])?.InnerText.Trim();
                DateOnly.TryParse(publicationDateString, CultureInfo.GetCultureInfo("ru-RU"), out DateOnly publicationDate);

                Vacancy vacancy = new Vacancy()
                {
                    WebSite = WebSiteName,
                    Link = link,
                    JobTitle = jobTitle,
                    Company = company,

                    Salary = vacancyNode.SelectSingleNode(this.configuration["Recruitika:XPaths:Salary"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim(),

                    JobType = this.GetJobType(vacancyNode),

                    Location = vacancyNode.SelectSingleNode(this.configuration["Recruitika:XPaths:Location"])?.ChildNodes[1].InnerText
                        .Replace("\n", string.Empty)
                        .Trim(),

                    Description = null,

                    PublicationDate = publicationDate,
                };
                vacancies.Add(vacancy);
            }

            return vacancies;
        }

        private int? GetNumberOfPages(HtmlNode document)
        {
            int? numberOfPages = null;
            var paginationNode = document.SelectSingleNode(this.configuration["Recruitika:XPaths:Pagination"]);

            if (paginationNode != null &&
                paginationNode.HasChildNodes)
            {
                var pagesString = paginationNode.LastChild?.FirstChild?.InnerText.Trim();
                int.TryParse(pagesString, out int result);
                numberOfPages = result;
            }

            return numberOfPages;
        }

        private async Task<IEnumerable<string>> LoadAdditionalPagesAsync(IRecruitikaHtmlLoader recruitikaHtmlLoader, string requstPath, int numberOfPages, CancellationToken token)
        {
            List<Task<string>> pagesTasks = new();

            for (int page = 2; page <= numberOfPages; page++)
            {
                string requestString = this.configuration["Recruitika:Domain"] + $"page/{page}/" + "?" + requstPath.Split('?').Last();
                var pageTask = recruitikaHtmlLoader.LoadJobBoardHTMLAsync(requestString, token);
                pagesTasks.Add(pageTask!);
            }

            string[] pages;

            try
            {
                pages = await Task.WhenAll(pagesTasks);
            }
            catch (HttpRequestException ex)
            {
                this.logger.LogError(ex, $"Can't load additional pages for {nameof(JobBoards.Recruitika)}");
                return Enumerable.Empty<string>();
            }

            return pages.ToList();
        }

        private string? GetJobType(HtmlNode vacancyNode)
        {
            string? jobType = null;

            if (vacancyNode != null)
            {
                var vacancyInfoNodes = vacancyNode.SelectSingleNode(this.configuration["Recruitika:XPaths:JobType"])?.ChildNodes;

                if (vacancyInfoNodes != null)
                {
                    foreach (var node in vacancyInfoNodes)
                    {
                        string? innerText = node?.FirstChild?.InnerText?.Trim();

                        if (innerText == "Remote")
                        {
                            jobType = "Тільки віддалено";
                        }
                    }
                }
            }

            return jobType;
        }
    }
}

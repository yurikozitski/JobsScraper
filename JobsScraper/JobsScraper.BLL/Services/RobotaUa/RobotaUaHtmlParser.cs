using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Interfaces.RobotaUa;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JobsScraper.BLL.Services.RobotaUa
{
    public class RobotaUaHtmlParser : IRobotaUaHtmlParser
    {
        private const string WebSiteName = "Robota.ua";

        private readonly IRobotaUaRequestStringBuilder robotaUaRequestStringBuilder;
        private readonly IRobotaUaHtmlLoader robotaUaHtmlLoader;
        private readonly IConfiguration configuration;
        private readonly ILogger<RobotaUaHtmlParser> logger;

        public RobotaUaHtmlParser(
            IRobotaUaRequestStringBuilder robotaUaRequestStringBuilder,
            IRobotaUaHtmlLoader raobotaUaHtmlLoader,
            IConfiguration configuration,
            ILogger<RobotaUaHtmlParser> logger)
        {
            this.robotaUaRequestStringBuilder = robotaUaRequestStringBuilder;
            this.robotaUaHtmlLoader = raobotaUaHtmlLoader;
            this.configuration = configuration;
            this.logger = logger;
        }

        public Task<IEnumerable<Vacancy>> ParseJobBoardHTMLAsync(string? jobBoardHTML, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(jobBoardHTML))
                {
                    this.logger.LogError($"Attempt to parse null or empty html string from {nameof(JobBoards.RobotaUa)}");
                    return Enumerable.Empty<Vacancy>();
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(jobBoardHTML);

                var vacancyNodesParent = doc.DocumentNode.SelectSingleNode(this.configuration["RobotaUa:XPaths:VacancyListParent"]);

                if (vacancyNodesParent == null)
                {
                    string message = $"Can't get vacancy nodes parent from {nameof(JobBoards.RobotaUa)}, XPath: {this.configuration["RobotaUa:XPaths:VacancyListParent"]}";
                    this.logger.LogError(message);
                    return Enumerable.Empty<Vacancy>();
                }

                var vacancyNodes = vacancyNodesParent.SelectNodes(this.configuration["RobotaUa:XPaths:VacancyList"]);

                if (vacancyNodes == null)
                {
                    string message = $"Can't get vacancy nodes from {nameof(JobBoards.RobotaUa)}, XPath: {this.configuration["RobotaUa:XPaths:VacancyList"]}";
                    this.logger.LogError(message);
                    return Enumerable.Empty<Vacancy>();
                }

                var vacancies = this.GetVacancyList(vacancyNodes, token);

                int? numberOfAdditionalPages = this.GetNumberOfPages(doc.DocumentNode);

                if (numberOfAdditionalPages != null)
                {
                    var additionalPages = await this.LoadAdditionalPagesAsync((int)numberOfAdditionalPages, token);

                    foreach (string page in additionalPages)
                    {
                        var pageDoc = new HtmlDocument();
                        pageDoc.LoadHtml(page);

                        var vacancyParentNode = pageDoc.DocumentNode.SelectSingleNode(this.configuration["RobotaUa:XPaths:VacancyListParent"]);

                        if (vacancyParentNode == null)
                        {
                            string message = @$"Can't get vacancy nodes parent from {nameof(JobBoards.RobotaUa)} for additional page, 
                                XPath: {this.configuration["RobotaUa:XPaths:VacancyListParent"]}";
                            this.logger.LogError(message);
                            continue;
                        }

                        var vacancyNodesAdditional = vacancyParentNode.SelectNodes(this.configuration["RobotaUa:XPaths:VacancyList"]);

                        if (vacancyNodesAdditional == null)
                        {
                            string message = @$"Can't get vacancy nodes from {nameof(JobBoards.RobotaUa)} for additional page, 
                                XPath: {this.configuration["RobotaUa:XPaths:VacancyListParent"]}";
                            this.logger.LogError(message);
                            continue;
                        }

                        vacancies.AddRange(this.GetVacancyList(vacancyNodesAdditional, token));
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

                string? localLink = vacancyNode.FirstChild?.Attributes["href"]?.Value?.Trim();

                if (localLink == null)
                {
                    var message = $"Can't parse local link from {nameof(JobBoards.RobotaUa)}";
                    this.logger.LogError(message);
                    continue;
                }

                string link = this.configuration["RobotaUa:ShortDomain"] + localLink;

                string jobTitle = vacancyNode.SelectSingleNode(this.configuration["RobotaUa:XPaths:JobTitle"])?.InnerText
                        .Replace("\n", " ")
                        .Trim()!;

                if (jobTitle == null)
                {
                    var message = $"Can't parse job title from {nameof(JobBoards.RobotaUa)}, XPath is {this.configuration["RobotaUa:XPaths:JobTitle"]}";
                    this.logger.LogError(message);
                    continue;
                }

                string company = null!;
                try
                {
                    company = vacancyNode.FirstChild?.LastChild?.ChildNodes[3].FirstChild?.ChildNodes[2]?.FirstChild?.InnerText
                            .Replace("\n", string.Empty)
                            .Trim()!;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex.Message);
                    continue;
                }

                if (company == null)
                {
                    var message = $"Can't parse company name from {nameof(JobBoards.RobotaUa)}, XPath is {this.configuration["RobotaUa:XPaths:Company"]}";
                    this.logger.LogError(message);
                    continue;
                }

                DateOnly? publicationDate = null;
                try
                {
                    string? publicationDateString = vacancyNode.FirstChild?.LastChild?.ChildNodes[9]?.ChildNodes[1]?.InnerText?.Trim();
                    publicationDate = GetDate(publicationDateString);
                }
                catch { }

                string? salary = null;
                try
                {
                    salary = vacancyNode.FirstChild?.LastChild?.ChildNodes[3]?.FirstChild.ChildNodes[1].ChildNodes[1].InnerText?
                        .Replace("\n", string.Empty)
                        .Replace("&nbsp;", string.Empty)
                        .Trim();
                }
                catch { }

                string? location = null;
                try
                {
                    location = vacancyNode.FirstChild?.LastChild?.ChildNodes[3].FirstChild?.ChildNodes[2]?.ChildNodes[5]?.InnerText
                            .Replace("\n", string.Empty)
                            .Trim()!;
                }
                catch { }

                Vacancy vacancy = new Vacancy()
                {
                    WebSite = WebSiteName,
                    Link = link,
                    JobTitle = jobTitle,
                    Company = company,

                    Salary = !string.IsNullOrWhiteSpace(salary) ? salary : null,

                    JobType = GetJobType(vacancyNode),

                    Location = location,

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
            var paginationNode = document.SelectSingleNode(this.configuration["RobotaUa:XPaths:Pagination"]);

            if (paginationNode != null &&
                paginationNode.HasChildNodes)
            {
                var pageString = paginationNode.ChildNodes?
                    .LastOrDefault(x => x.Name == "a" && Regex.IsMatch(x.InnerText.Trim(), @"^\d+$"))?.InnerText?.Trim();
                int.TryParse(pageString, out int result);
                numberOfPages = result;
            }

            return numberOfPages;
        }

        private async Task<IEnumerable<string>> LoadAdditionalPagesAsync(int numberOfPages, CancellationToken token)
        {
            List<Task<string>> pagesTasks = new();

            string requestString = this.robotaUaRequestStringBuilder.RequestString;

            if (requestString.Contains("?"))
            {
                requestString += "&";
            }
            else
            {
                requestString += "?";
            }

            for (int page = 2; page <= numberOfPages; page++)
            {
                var pageTask = this.robotaUaHtmlLoader.LoadJobBoardHTMLAsync(requestString + $"page={page}", token);
                pagesTasks.Add(pageTask!);
            }

            string[] pages;

            try
            {
                pages = await Task.WhenAll(pagesTasks);
            }
            catch (HttpRequestException ex)
            {
                this.logger.LogError(ex, $"Can't load additional pages for {nameof(JobBoards.RobotaUa)}");
                return Enumerable.Empty<string>();
            }

            return pages.ToList();
        }

        private static string? GetJobType(HtmlNode vacancyNode)
        {
            if (vacancyNode != null)
            {
                string? innerText = null;
                try
                {
                    innerText = vacancyNode.FirstChild?.LastChild?.FirstChild?.ChildNodes[6]?.FirstChild?.InnerText.Trim();
                }
                catch { }

                if (innerText != null &&
                    string.Equals(innerText, "Віддалена робота", StringComparison.InvariantCultureIgnoreCase))
                    return "Тільки віддалено";

                bool? isOffice = false;
                try
                {
                    isOffice = vacancyNode.FirstChild?.LastChild?.ChildNodes[5]?.ChildNodes?
                        .Any(x => x.InnerText.Contains("Офіс", StringComparison.InvariantCultureIgnoreCase));
                }
                catch { }

                if ((bool)isOffice!)
                    return "Тільки офіс";

                bool? isHybrid = false;
                try
                {
                    isHybrid = vacancyNode.FirstChild?.LastChild?.ChildNodes[5]?.ChildNodes?
                        .Any(x => x.InnerText.Contains("Гібрид", StringComparison.InvariantCultureIgnoreCase));
                }
                catch { }

                if ((bool)isHybrid!)
                    return "Гібридна робота";
            }

            return null;
        }

        private static DateOnly? GetDate(string? publicationDateString)
        {
            if (publicationDateString == null)
                return null;

            if(publicationDateString.Contains("тиж", StringComparison.InvariantCultureIgnoreCase) ||
                publicationDateString.Contains("місяц", StringComparison.InvariantCultureIgnoreCase))
                return null;

            DateOnly? date = null;

            if(publicationDateString.Contains("годин", StringComparison.InvariantCultureIgnoreCase))
            {
                string hoursStr = default!;

                for (int i = 0; i < publicationDateString.Length; i++)
                {
                    char chr = publicationDateString[i];
                    if (char.IsDigit(chr))
#pragma warning disable S1643 // Strings should not be concatenated using '+' in a loop
                        hoursStr += chr;
                }

                int hours = int.Parse(hoursStr);
                var now = DateTime.UtcNow.AddHours(2).AddHours(-hours);
                date = DateOnly.FromDateTime(now);
                return date;
            }

            if (publicationDateString.Contains("днів", StringComparison.InvariantCultureIgnoreCase) ||
                publicationDateString.Contains("день", StringComparison.InvariantCultureIgnoreCase))
            {
                string daysStr = default!;

                for (int i = 0; i < publicationDateString.Length; i++)
                {
                    char chr = publicationDateString[i];
                    if (char.IsDigit(chr))
                        daysStr += chr;
                }

                int days = int.Parse(daysStr);
                var now = DateTime.UtcNow.AddHours(2).AddDays(-days);
                date = DateOnly.FromDateTime(now);
                return date;
            }

            return date;
        }
    }
}

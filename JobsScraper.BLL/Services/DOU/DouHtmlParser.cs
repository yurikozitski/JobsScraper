using HtmlAgilityPack;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.BLL.Services.DOU
{
    public class DouHtmlParser : IDouHtmlParser
    {
        private const string WebSiteName = "DOU";
        private readonly IConfiguration configuration;

        public DouHtmlParser(
            IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task<IEnumerable<Vacancy>> ParseJobBoardHTMLAsync(string jobBoardHTML, CancellationToken token)
        {
            return Task.Run(() =>
            {
                if (jobBoardHTML == null)
                {
                    return Enumerable.Empty<Vacancy>();
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(jobBoardHTML);

                var vacancyNodes = doc.DocumentNode.SelectNodes(this.configuration["DOU:XPaths:VacancyList"]);

                if (vacancyNodes == null)
                {
                    return Enumerable.Empty<Vacancy>();
                }

                var vacancies = GetVacancyList(vacancyNodes, this.configuration, token);

                return vacancies;
            });
        }

        private static List<Vacancy> GetVacancyList(HtmlNodeCollection vacancyNodes, IConfiguration configuration, CancellationToken token)
        {
            List<Vacancy> vacancies = new();

            foreach (var vacancyNode in vacancyNodes)
            {
                token.ThrowIfCancellationRequested();

                string? link = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:LinkTitle"])?.Attributes["href"]?.Value
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (link == null)
                {
                    var message = $"Can't parse local link from {nameof(JobBoards.Dou)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string jobTitle = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:LinkTitle"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (jobTitle == null)
                {
                    var message = $"Can't parse job title from {nameof(JobBoards.Dou)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string company = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:Company"])?.InnerText
                        .Replace("&nbsp;", string.Empty)
                        .Replace("\n", string.Empty)
                        .Trim()!;

                if (company == null)
                {
                    var message = $"Can't parse company name from {nameof(JobBoards.Dou)}, XPath is";
                    Console.WriteLine(message);
                    continue;
                }

                string? publicationDateString = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:PublicationDate"])?.InnerText.Trim();
                DateOnly? publicationDate = null;

                if (publicationDateString != null)
                {
                    int.TryParse(publicationDateString.Split(' ').FirstOrDefault(), out int publicationDay);
                    var publicationMonthString = publicationDateString.Split(' ').LastOrDefault();

                    int publicationMonth = publicationMonthString switch
                    {
                        "січня" => 1,
                        "лютого" => 2,
                        "березня" => 3,
                        "квітня" => 4,
                        "травня" => 5,
                        "червня" => 6,
                        "липня" => 7,
                        "серпня" => 8,
                        "вересня" => 9,
                        "жовтня" => 10,
                        "листопада" => 11,
                        "грудня" => 12,
                        _ => 1
                    };

                    publicationDate = new DateOnly(DateTime.Now.Year, publicationMonth, publicationDay);
                }

                string? location = null;
                var locationStrings = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:JobTypeLocation"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Split(',')
                        .Select(x => x.Trim())
                        .Where(x => x != "віддалено");

                if (locationStrings != null)
                {
                    location = string.Join(",", locationStrings);
                }

                Vacancy vacancy = new Vacancy()
                {
                    WebSite = WebSiteName,
                    Link = link,
                    JobTitle = jobTitle,
                    Company = company,

                    Salary = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:Salary"])?.InnerText
                        .Replace("\n", string.Empty)
                        .Trim(),

                    JobType = GetJobType(vacancyNode, configuration),

                    Location = location,

                    Description = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:Description"])?.InnerText
                        .Replace("&nbsp;", " ")
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

        private static string? GetJobType(HtmlNode vacancyNode, IConfiguration configuration)
        {
            string? jobType = null;

            if (vacancyNode != null)
            {
                var jobTypeLocations = vacancyNode.SelectSingleNode(configuration["DOU:XPaths:JobTypeLocation"])?.InnerText
                    .Replace("\n", string.Empty)
                    .Split(',')
                    .Select(x => x.Trim());

                if (jobTypeLocations != null)
                {
                    if (jobTypeLocations.Count() == 1 && jobTypeLocations.First() == "віддалено")
                    {
                        jobType = "Тільки віддалено";
                    }
                    else if (jobTypeLocations.Last() == "віддалено")
                    {
                        jobType = "Office або Remote";
                    }
                    else if (!jobTypeLocations.Contains("віддалено"))
                    {
                        jobType = "Тільки офіс";
                    }
                }
            }

            return jobType;
        }
    }
}

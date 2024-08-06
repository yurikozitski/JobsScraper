using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Services.DOU;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.Tests.DOU
{
    public class DouRequestStringBuilderTests
    {

#pragma warning disable S3263 // Static fields should appear in the order they must be initialized 
        private static IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
#pragma warning restore S3263 // Static fields should appear in the order they must be initialized 

        private static Dictionary<string, string> configData = new Dictionary<string, string>
        {
            { "DOU:Domain", "https://jobs.dou.ua/vacancies/" },
        };

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionValid))]
        public void GetRequestString_ValidData_ReturnsRequestString(JobSearchModel jobSearchModel, string expected)
        {
            // Arrange
            var douRequestStringBuilder = new DouRequestStringBuilder(configuration);

            // Act
            bool isEqual = string.Equals(douRequestStringBuilder.GetRequestString(jobSearchModel), expected, StringComparison.InvariantCulture);

            // Assert
            Assert.True(isEqual);
        }

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionInValid))]
        public void GetRequestString_InValidData_ThrowsException(JobSearchModel jobSearchModel)
        {
            // Arrange
            var douRequestStringBuilder = new DouRequestStringBuilder(configuration);

            // Act
            var result = douRequestStringBuilder.GetRequestString;

            // Assert
            Assert.ThrowsAny<ArgumentException>(() => result(jobSearchModel));
        }

        public static IEnumerable<object[]> JobSearchModelsCollectionValid =>
            new List<object[]>()
            {
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                    },
                    configuration["DOU:Domain"]! + "?category=.NET",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.Fullstack,
                    },
                    configuration["DOU:Domain"]! + "?search=Full Stack",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                    },
                    configuration["DOU:Domain"]! + "?remote&category=.NET",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.Fullstack,
                        Grade = Grades.Junior,
                    },
                    configuration["DOU:Domain"]! + "?search=Full Stack+Junior",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                        Grade = Grades.Junior,
                    },
                    configuration["DOU:Domain"]! + "?remote&category=.NET&search=Junior",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                        Country = Countries.Ukraine,
                        City = Cities.Kyiv,
                        ExperienceLevel = ExperienceLevels.OneYear,
                        Grade = Grades.Junior,
                        SalaryFrom = 500,
                        EnglishLevel = EnglishLevels.Intermediate,
                    },
                    configuration["DOU:Domain"]!
                    + "?remote&city=Київ&exp=1-3&category=.NET&search=Україна+Junior",
                },
            };

        public static IEnumerable<object[]> JobSearchModelsCollectionInValid =>
            new List<object[]>()
            {
                new object[]
                {
                    null!,
                },
            };
    }
}

using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Services.Recruitika;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.Tests.Recruitika
{
    public class RecruitikaRequestStringBuilderTests
    {

#pragma warning disable S3263 // Static fields should appear in the order they must be initialized 
        private static IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
#pragma warning restore S3263 // Static fields should appear in the order they must be initialized 

        private static Dictionary<string, string> configData = new Dictionary<string, string>
        {
            { "Recruitika:Domain", "https://recruitika.com/" },
        };

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionValid))]
        public void GetRequestString_ValidData_ReturnsRequestString(JobSearchModel jobSearchModel, string expected)
        {
            // Arrange
            var recruitikaRequestStringBuilder = new RecruitikaRequestStringBuilder(configuration);

            // Act
            string actual = recruitikaRequestStringBuilder.GetRequestString(jobSearchModel);

            // Assert
            Assert.True(string.Equals(actual, expected, StringComparison.InvariantCulture));
        }

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionInValid))]
        public void GetRequestString_InValidData_ThrowsException(JobSearchModel jobSearchModel)
        {
            // Arrange
            var recruitikaRequestStringBuilder = new RecruitikaRequestStringBuilder(configuration);

            // Act
            var result = recruitikaRequestStringBuilder.GetRequestString;

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
                    configuration["Recruitika:Domain"]! + "?tags=net",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                    },
                    configuration["Recruitika:Domain"]! + "?tags=net&collections=remote",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.Fullstack,
                        Grade = Grades.Junior,
                    },
                    configuration["Recruitika:Domain"]! + "?tags=full-stack&collections=juniorfriendly",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                        Grade = Grades.Junior,
                    },
                    configuration["Recruitika:Domain"]! + "?tags=net&collections=remote,juniorfriendly",
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
                    configuration["Recruitika:Domain"]!
                    + "?tags=net&collections=remote,juniorfriendly&cities=kyiv",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                        Country = Countries.Ukraine,
                        City = Cities.Kyiv | Cities.Lviv,
                        ExperienceLevel = ExperienceLevels.NoExperience,
                        Grade = Grades.Junior,
                        SalaryFrom = 300,
                        EnglishLevel = EnglishLevels.Intermediate,
                    },
                    configuration["Recruitika:Domain"]!
                    + "?tags=net&collections=remote,juniorfriendly&cities=kyiv,lvov",
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

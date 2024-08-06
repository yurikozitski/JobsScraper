using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Services.Djinni;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.Tests.Djinni
{
    public class DjinniRequestStringBuilderTests
    {
#pragma warning disable S3263 // Static fields should appear in the order they must be initialized 
        private static IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
#pragma warning restore S3263 // Static fields should appear in the order they must be initialized 

        private static Dictionary<string, string> configData = new Dictionary<string, string>
        {
            { "Djinni:Domain", "https://djinni.co/jobs/" },
        };

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionValid))]
        public void GetRequestString_ValidData_ReturnsRequestString(JobSearchModel jobSearchModel, string expected)
        {
            // Arrange
            var djinniRequestStringBuilder = new DjinniRequestStringBuilder(configuration);

            // Act
            bool isEqual = string.Equals(djinniRequestStringBuilder.GetRequestString(jobSearchModel), expected, StringComparison.InvariantCulture);

            // Assert
            Assert.True(isEqual);
        }

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionInValid))]
        public void GetRequestString_InValidData_ThrowsException(JobSearchModel jobSearchModel)
        {
            // Arrange
            var djinniRequestStringBuilder = new DjinniRequestStringBuilder(configuration);

            // Act
            var result = djinniRequestStringBuilder.GetRequestString;

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
                    configuration["Djinni:Domain"]! + "?primary_keyword=.NET",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                    },
                    configuration["Djinni:Domain"]! + "?primary_keyword=.NET&employment=remote",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                        Grade = Grades.Junior,
                    },
                    configuration["Djinni:Domain"]! + "?primary_keyword=.NET&employment=remote&exp_rank=junior",
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
                    configuration["Djinni:Domain"]! 
                    + "?primary_keyword=.NET&employment=remote&region=UKR&location=kyiv&exp_level=1y&exp_rank=junior&salary=500&english_level=intermediate",
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

using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Services.RobotaUa;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.Tests.RobotaUa
{
    public class RobotaUaRequestStringBuilderTests
    {

#pragma warning disable S3263 // Static fields should appear in the order they must be initialized 
        private static IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
#pragma warning restore S3263 // Static fields should appear in the order they must be initialized 

        private static Dictionary<string, string> configData = new Dictionary<string, string>
        {
            { "RobotaUa:Domain", "https://robota.ua/zapros/" },
        };

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionValid))]
        public void GetRequestString_ValidData_ReturnsRequestString(JobSearchModel jobSearchModel, string expected)
        {
            // Arrange
            var robotaUaRequestStringBuilder = new RobotaUaRequestStringBuilder(configuration);

            // Act
            string actual = robotaUaRequestStringBuilder.GetRequestString(jobSearchModel);

            // Assert
            Assert.True(string.Equals(actual, expected, StringComparison.InvariantCulture));
        }

        [Theory]
        [MemberData(nameof(JobSearchModelsCollectionInValid))]
        public void GetRequestString_InValidData_ThrowsException(JobSearchModel jobSearchModel)
        {
            // Arrange
            var robotaUaRequestStringBuilder = new RobotaUaRequestStringBuilder(configuration);

            // Act
            var result = robotaUaRequestStringBuilder.GetRequestString;

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
                    configuration["RobotaUa:Domain"]! + ".NET/ukraine",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                    },
                    configuration["RobotaUa:Domain"]! + ".NET/ukraine?scheduleIds=3",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.Fullstack,
                        Grade = Grades.Junior,
                    },
                    configuration["RobotaUa:Domain"]! + "Full-Stack-junior/ukraine",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                        Grade = Grades.Junior,
                    },
                    configuration["RobotaUa:Domain"]! + ".NET-junior/ukraine?scheduleIds=3",
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
                    configuration["RobotaUa:Domain"]!
                    + ".NET-junior/kyiv?scheduleIds=3&salary=20000",
                },
                new object[]
                {
                    new JobSearchModel()
                    {
                        JobStack = JobStacks.CSharpDotNET,
                        JobType = JobTypes.Remote,
                        Country = Countries.Ukraine,
                        City = Cities.Kyiv,
                        ExperienceLevel = ExperienceLevels.NoExperience,
                        Grade = Grades.Junior,
                        SalaryFrom = 300,
                        EnglishLevel = EnglishLevels.Intermediate,
                    },
                    configuration["RobotaUa:Domain"]!
                    + ".NET-junior/kyiv?scheduleIds=3&salary=12000&experienceType=true",
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

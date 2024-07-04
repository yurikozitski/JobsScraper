using System.Text;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Extensions;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.BLL.Services.DOU
{
    public class DouRequestStringBuilder : IDouRequestStringBuilder
    {
        private readonly IConfiguration configuration;

        public DouRequestStringBuilder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetRequestString(JobSearchModel jobSearchModel)
        {
            StringBuilder requestStringBuilder = new StringBuilder(this.configuration["DOU:Domain"]);

            AddJobStackPath(requestStringBuilder, jobSearchModel.JobStack);
            AddJobTypesPath(requestStringBuilder, jobSearchModel.JobType);
            AddCountryPath(requestStringBuilder, jobSearchModel.Country);
            AddCityPath(requestStringBuilder, jobSearchModel.City);
            AddExperienceLevelPath(requestStringBuilder, jobSearchModel.ExperienceLevel);
            AddGradePath(requestStringBuilder, jobSearchModel.Grade);
            //AddSalaryPath(requestStringBuilder, jobSearchModel.SalaryFrom);
            //AddEnglishLevelsPath(requestStringBuilder, jobSearchModel.EnglishLevel);

            string requestString = requestStringBuilder.ToString();
            return requestString;
        }

        private static void AddJobStackPath(StringBuilder sb, JobStacks jobStacks)
        {
            if (jobStacks is JobStacks.CSharpDotNET ||
                jobStacks is JobStacks.Java ||
                jobStacks is JobStacks.PHP ||
                jobStacks is JobStacks.CCppEmbeddedSystem ||
                jobStacks is JobStacks.ProjectManager ||
                jobStacks is JobStacks.ProductManager ||
                jobStacks is JobStacks.HR ||
                jobStacks is JobStacks.Sales ||
                jobStacks is JobStacks.BusinessAnalyst ||
                jobStacks is JobStacks.Python ||
                jobStacks is JobStacks.Ruby ||
                jobStacks is JobStacks.Golang ||
                jobStacks is JobStacks.iOS ||
                jobStacks is JobStacks.Android ||
                jobStacks is JobStacks.JavaScriptFrontEnd ||
                jobStacks is JobStacks.NodeJs ||
                jobStacks is JobStacks.DesignUIUX ||
                jobStacks is JobStacks.Marketing ||
                jobStacks is JobStacks.DevOps)
            {
                sb.Append("?category=");
            }
            else
            {
                sb.Append("?search=");
            }

            sb.Append(jobStacks.ToQueryParam(JobBoards.Dou));
        }

        private static void AddJobTypesPath(StringBuilder sb, JobTypes? jobTypes)
        {
            if (jobTypes != null)
            {
                if (((JobTypes)jobTypes).HasFlag(JobTypes.Remote))
                    sb.Append("&remote");
            }
        }

        private static void AddCountryPath(StringBuilder sb, Countries? countries)
        {
            if (countries != null)
            {
                if (((Countries)countries).HasFlag(Countries.Ukraine))
                    sb.Append("&search=Україна");

                if (((Countries)countries).HasFlag(Countries.Poland))
                    sb.Append("&search=Польща");

                if (((Countries)countries).HasFlag(Countries.EU))
                    sb.Append("&search=Європа");

                if (((Countries)countries).HasFlag(Countries.Other))
                    sb.Append("&search=інші+країни");
            }
        }

        private static void AddCityPath(StringBuilder sb, Cities? cities)
        {
            if (cities != null)
            {
                if (((Cities)cities).HasFlag(Cities.Kyiv))
                    sb.Append("&city=Київ");

                if (((Cities)cities).HasFlag(Cities.Vinnytsia))
                    sb.Append("&city=Вінниця");

                if (((Cities)cities).HasFlag(Cities.Dnipro))
                    sb.Append("&city=Дніпро");

                if (((Cities)cities).HasFlag(Cities.IvanoFrankivsk))
                    sb.Append("&city=Івано-Франківськ");

                if (((Cities)cities).HasFlag(Cities.Zhytomyr))
                    sb.Append("&city=Житомир");

                if (((Cities)cities).HasFlag(Cities.Zaporizhzhia))
                    sb.Append("&city=Запоріжжя");

                if (((Cities)cities).HasFlag(Cities.Lviv))
                    sb.Append("&city=Львів");

                if (((Cities)cities).HasFlag(Cities.Mykolaiv))
                    sb.Append("&city=Миколаїв");

                if (((Cities)cities).HasFlag(Cities.Odesa))
                    sb.Append("&city=Одеса");

                if (((Cities)cities).HasFlag(Cities.Ternopil))
                    sb.Append("&city=Тернопіль");

                if (((Cities)cities).HasFlag(Cities.Kharkiv))
                    sb.Append("&city=Харків");

                if (((Cities)cities).HasFlag(Cities.Khmelnytskyi))
                    sb.Append("&city=Хмельницький");

                if (((Cities)cities).HasFlag(Cities.Cherkasy))
                    sb.Append("&city=Черкаси");

                if (((Cities)cities).HasFlag(Cities.Chernihiv))
                    sb.Append("&city=Чернігів");

                if (((Cities)cities).HasFlag(Cities.Chernivtsi))
                    sb.Append("&city=Чернівці");

                if (((Cities)cities).HasFlag(Cities.Uzhhorod))
                    sb.Append("&city=Ужгород");
            }
        }

        private static void AddExperienceLevelPath(StringBuilder sb, ExperienceLevels? experienceLevels)
        {
            if (experienceLevels != null)
            {
                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.NoExperience))
                    sb.Append("&exp=0-1");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.OneYear))
                    sb.Append("&exp=1-3");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.TwoYears))
                    sb.Append("&exp=1-3");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.ThreeYears))
                    sb.Append("&exp=3-5");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.FourYear))
                    sb.Append("&exp=3-5");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.FiveYearsAndAbove))
                    sb.Append("&exp=5plus");
            }
        }

        private static void AddGradePath(StringBuilder sb, Grades? grades)
        {
            if (grades != null)
            {
                if (((Grades)grades).HasFlag(Grades.TraineeIntern))
                    sb.Append("search=Trainee");

                if (((Grades)grades).HasFlag(Grades.Junior))
                    sb.Append("&search=Junior");

                if (((Grades)grades).HasFlag(Grades.Middle))
                    sb.Append("&search=Middle");

                if (((Grades)grades).HasFlag(Grades.Senior))
                    sb.Append("&search=Senior");

                if (((Grades)grades).HasFlag(Grades.TeamLead))
                    sb.Append("&search=Team Lead");

                if (((Grades)grades).HasFlag(Grades.HeadChief))
                    sb.Append("&search=Chief Head");
            }
        }

        private static void AddSalaryPath(StringBuilder sb, int? salary)
        {
            if (salary != null)
            {
                sb.Append($"&salary={salary}");
            }
        }

        private static void AddEnglishLevelsPath(StringBuilder sb, EnglishLevels? englishLevels)
        {
            if (englishLevels != null)
            {
                if (((EnglishLevels)englishLevels).HasFlag(EnglishLevels.NoEnglish))
                    sb.Append("&english_level=no_english");

                if (((EnglishLevels)englishLevels).HasFlag(EnglishLevels.Elementary))
                    sb.Append("&english_level=basic");

                if (((EnglishLevels)englishLevels).HasFlag(EnglishLevels.PreIntermediate))
                    sb.Append("&english_level=pre");

                if (((EnglishLevels)englishLevels).HasFlag(EnglishLevels.Intermediate))
                    sb.Append("&english_level=intermediate");

                if (((EnglishLevels)englishLevels).HasFlag(EnglishLevels.UpperIntermediate))
                    sb.Append("&english_level=upper");

                if (((EnglishLevels)englishLevels).HasFlag(EnglishLevels.Fluent))
                    sb.Append("&english_level=fluent");
            }
        }
    }
}

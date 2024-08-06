using System.Text;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Extensions;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.BLL.Services.Djinni
{
    public class DjinniRequestStringBuilder : IDjinniRequestStringBuilder
    {
        private readonly IConfiguration configuration;

        public string RequestString { get; private set; } = default!;

        public DjinniRequestStringBuilder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetRequestString(JobSearchModel jobSearchModel)
        {
            ArgumentNullException.ThrowIfNull(jobSearchModel);

            StringBuilder requestStringBuilder = new StringBuilder(this.configuration["Djinni:Domain"]);

            AddJobStackPath(requestStringBuilder, jobSearchModel.JobStack);
            AddJobTypesPath(requestStringBuilder, jobSearchModel.JobType);
            AddCountryPath(requestStringBuilder, jobSearchModel.Country);
            AddCityPath(requestStringBuilder, jobSearchModel.City);
            AddExperienceLevelPath(requestStringBuilder, jobSearchModel.ExperienceLevel);
            AddGradePath(requestStringBuilder, jobSearchModel.Grade);
            AddSalaryPath(requestStringBuilder, jobSearchModel.SalaryFrom);
            AddEnglishLevelsPath(requestStringBuilder, jobSearchModel.EnglishLevel);

            string requestString = requestStringBuilder.ToString();
            this.RequestString = requestString;

            return requestString;
        }

        private static void AddJobStackPath(StringBuilder sb, JobStacks jobStacks)
        {
            sb.Append("?primary_keyword=");
            sb.Append(jobStacks.ToQueryParam(JobBoards.Djinni));
        }

        private static void AddJobTypesPath(StringBuilder sb, JobTypes? jobTypes)
        {
            if (jobTypes != null)
            {
                if (((JobTypes)jobTypes).HasFlag(JobTypes.OnSite))
                    sb.Append("&employment=office");

                if (((JobTypes)jobTypes).HasFlag(JobTypes.Hybrid))
                    sb.Append("&employment=parttime");

                if (((JobTypes)jobTypes).HasFlag(JobTypes.Remote))
                    sb.Append("&employment=remote");
            }
        }

        private static void AddCountryPath(StringBuilder sb, Countries? countries)
        {
            if (countries != null)
            {
                if (((Countries)countries).HasFlag(Countries.Ukraine))
                    sb.Append("&region=UKR");

                if (((Countries)countries).HasFlag(Countries.Poland))
                    sb.Append("&region=POL");

                if (((Countries)countries).HasFlag(Countries.EU))
                    sb.Append("&region=eu");

                if (((Countries)countries).HasFlag(Countries.Other))
                    sb.Append("&region=other");
            }
        }

        private static void AddCityPath(StringBuilder sb, Cities? cities)
        {
            if (cities != null)
            {
                if (((Cities)cities).HasFlag(Cities.Kyiv))
                    sb.Append("&location=kyiv");

                if (((Cities)cities).HasFlag(Cities.Vinnytsia))
                    sb.Append("&location=vinnytsia");

                if (((Cities)cities).HasFlag(Cities.Dnipro))
                    sb.Append("&location=dnipro");

                if (((Cities)cities).HasFlag(Cities.IvanoFrankivsk))
                    sb.Append("&location=ivanofrankivsk");

                if (((Cities)cities).HasFlag(Cities.Zhytomyr))
                    sb.Append("&location=zhytomyr");

                if (((Cities)cities).HasFlag(Cities.Zaporizhzhia))
                    sb.Append("&location=zaporizhzhia");

                if (((Cities)cities).HasFlag(Cities.Lviv))
                    sb.Append("&location=lviv");

                if (((Cities)cities).HasFlag(Cities.Mykolaiv))
                    sb.Append("&location=mykolaiv");

                if (((Cities)cities).HasFlag(Cities.Odesa))
                    sb.Append("&location=odesa");

                if (((Cities)cities).HasFlag(Cities.Ternopil))
                    sb.Append("&location=ternopil");

                if (((Cities)cities).HasFlag(Cities.Kharkiv))
                    sb.Append("&location=kharkiv");

                if (((Cities)cities).HasFlag(Cities.Khmelnytskyi))
                    sb.Append("&location=khmelnytskyi");

                if (((Cities)cities).HasFlag(Cities.Cherkasy))
                    sb.Append("&location=cherkasy");

                if (((Cities)cities).HasFlag(Cities.Chernihiv))
                    sb.Append("&location=chernihiv");

                if (((Cities)cities).HasFlag(Cities.Chernivtsi))
                    sb.Append("&location=chernivtsi");

                if (((Cities)cities).HasFlag(Cities.Uzhhorod))
                    sb.Append("&location=uzhhorod");
            }
        }

        private static void AddExperienceLevelPath(StringBuilder sb, ExperienceLevels? experienceLevels)
        {
            if (experienceLevels != null)
            {
                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.NoExperience))
                    sb.Append("&exp_level=no_exp");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.OneYear))
                    sb.Append("&exp_level=1y");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.TwoYears))
                    sb.Append("&exp_level=2y");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.ThreeYears))
                    sb.Append("&exp_level=3y");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.FourYear))
                    sb.Append("&exp_level=4y");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.FiveYearsAndAbove))
                    sb.Append("&exp_level=5y");
            }
        }

        private static void AddGradePath(StringBuilder sb, Grades? grades)
        {
            if (grades != null)
            {
                if (((Grades)grades).HasFlag(Grades.TraineeIntern))
                    sb.Append("&exp_rank=trainee_intern");

                if (((Grades)grades).HasFlag(Grades.Junior))
                    sb.Append("&exp_rank=junior");

                if (((Grades)grades).HasFlag(Grades.Middle))
                    sb.Append("&exp_rank=middle");

                if (((Grades)grades).HasFlag(Grades.Senior))
                    sb.Append("&exp_rank=senior");

                if (((Grades)grades).HasFlag(Grades.TeamLead))
                    sb.Append("&exp_rank=team_lead");

                if (((Grades)grades).HasFlag(Grades.HeadChief))
                    sb.Append("&exp_rank=chief_head");
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

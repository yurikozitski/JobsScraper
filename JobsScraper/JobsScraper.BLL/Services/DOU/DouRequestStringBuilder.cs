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
        private bool hasSearch;
        private bool hasQueryParams;

        public DouRequestStringBuilder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetRequestString(JobSearchModel jobSearchModel)
        {
            ArgumentNullException.ThrowIfNull(jobSearchModel);

            StringBuilder requestStringBuilder = new StringBuilder(this.configuration["DOU:Domain"]);

            this.AddJobTypesPath(requestStringBuilder, jobSearchModel.JobType);
            this.AddCityPath(requestStringBuilder, jobSearchModel.City);
            this.AddExperienceLevelPath(requestStringBuilder, jobSearchModel.ExperienceLevel);
            this.AddJobStackPath(requestStringBuilder, jobSearchModel.JobStack);
            this.AddCountryPath(requestStringBuilder, jobSearchModel.Country);
            this.AddGradePath(requestStringBuilder, jobSearchModel.Grade);

            string requestString = requestStringBuilder.ToString();
            return requestString;
        }

        private void AddJobStackPath(StringBuilder sb, JobStacks jobStacks)
        {
            string prefix = this.hasQueryParams ? "&" : "?";

            if (jobStacks is JobStacks.CSharpDotNET ||
                jobStacks is JobStacks.Java ||
                jobStacks is JobStacks.JavaScriptFrontEnd ||
                jobStacks is JobStacks.Python
                //jobStacks is JobStacks.PHP 
                //jobStacks is JobStacks.CCppEmbeddedSystem ||
                //jobStacks is JobStacks.ProjectManager ||
                //jobStacks is JobStacks.ProductManager ||
                //jobStacks is JobStacks.HR ||
                //jobStacks is JobStacks.Sales ||
                //jobStacks is JobStacks.BusinessAnalyst ||
                //jobStacks is JobStacks.Ruby ||
                //jobStacks is JobStacks.Golang ||
                //jobStacks is JobStacks.iOS ||
                //jobStacks is JobStacks.Android ||
                //jobStacks is JobStacks.NodeJs ||
                //jobStacks is JobStacks.DesignUIUX ||
                //jobStacks is JobStacks.Marketing ||
                //jobStacks is JobStacks.DevOps
                )
            {
                sb.Append($"{prefix}category=");
                this.hasQueryParams = true;
            }
            else
            {
                sb.Append($"{prefix}search=");
                this.hasQueryParams = true;
                this.hasSearch = true;
            }

            sb.Append(jobStacks.ToQueryParam(JobBoards.Dou));
        }

        private void AddJobTypesPath(StringBuilder sb, JobTypes? jobTypes)
        {
            if (jobTypes != null)
            {
                if (((JobTypes)jobTypes).HasFlag(JobTypes.Remote))
                {
                    sb.Append("?remote");
                    this.hasQueryParams = true;
                }
            }
        }

        private void AddCountryPath(StringBuilder sb, Countries? countries)
        {
            if (countries != null)
            {
                string prefix = string.Empty;

                if (this.hasSearch && this.hasQueryParams)
                {
                    prefix = "+";
                }

                if (!this.hasSearch && this.hasQueryParams)
                {
                    prefix = "&search=";
                    this.hasSearch = true;
                }

                if (!this.hasSearch && !this.hasQueryParams)
                {
                    prefix = "?search=";
                    this.hasQueryParams = true;
                }

                if (((Countries)countries).HasFlag(Countries.Ukraine))
                    sb.Append($"{prefix}Україна");

                if (((Countries)countries).HasFlag(Countries.Poland))
                    sb.Append($"{prefix}Польща");

                if (((Countries)countries).HasFlag(Countries.EU))
                    sb.Append($"{prefix}Європа");

                if (((Countries)countries).HasFlag(Countries.Other))
                    sb.Append($"{prefix}інші+країни");
            }
        }

        private void AddCityPath(StringBuilder sb, Cities? cities)
        {
            if (cities != null)
            {
                string prefix = this.hasQueryParams ? "&" : "?";

                if (((Cities)cities).HasFlag(Cities.Kyiv))
                    sb.Append($"{prefix}city=Київ");

                if (((Cities)cities).HasFlag(Cities.Vinnytsia))
                    sb.Append($"{prefix}city=Вінниця");

                if (((Cities)cities).HasFlag(Cities.Dnipro))
                    sb.Append($"{prefix}city=Дніпро");

                if (((Cities)cities).HasFlag(Cities.IvanoFrankivsk))
                    sb.Append($"{prefix}city=Івано-Франківськ");

                if (((Cities)cities).HasFlag(Cities.Zhytomyr))
                    sb.Append($"{prefix}city=Житомир");

                if (((Cities)cities).HasFlag(Cities.Zaporizhzhia))
                    sb.Append($"{prefix}city=Запоріжжя");

                if (((Cities)cities).HasFlag(Cities.Lviv))
                    sb.Append($"{prefix}city=Львів");

                if (((Cities)cities).HasFlag(Cities.Mykolaiv))
                    sb.Append($"{prefix}city=Миколаїв");

                if (((Cities)cities).HasFlag(Cities.Odesa))
                    sb.Append($"{prefix}city=Одеса");

                if (((Cities)cities).HasFlag(Cities.Ternopil))
                    sb.Append($"{prefix}city=Тернопіль");

                if (((Cities)cities).HasFlag(Cities.Kharkiv))
                    sb.Append($"{prefix}city=Харків");

                if (((Cities)cities).HasFlag(Cities.Khmelnytskyi))
                    sb.Append($"{prefix}city=Хмельницький");

                if (((Cities)cities).HasFlag(Cities.Cherkasy))
                    sb.Append($"{prefix}city=Черкаси");

                if (((Cities)cities).HasFlag(Cities.Chernihiv))
                    sb.Append($"{prefix}city=Чернігів");

                if (((Cities)cities).HasFlag(Cities.Chernivtsi))
                    sb.Append($"{prefix}city=Чернівці");

                if (((Cities)cities).HasFlag(Cities.Uzhhorod))
                    sb.Append($"{prefix}city=Ужгород");

                this.hasQueryParams = true;
            }
        }

        private void AddExperienceLevelPath(StringBuilder sb, ExperienceLevels? experienceLevels)
        {
            if (experienceLevels != null)
            {
                string prefix = this.hasQueryParams ? "&" : "?";

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.NoExperience))
                    sb.Append($"{prefix}exp=0-1");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.OneYear))
                    sb.Append($"{prefix}exp=1-3");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.TwoYears))
                    sb.Append($"{prefix}exp=1-3");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.ThreeYears))
                    sb.Append($"{prefix}exp=3-5");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.FourYear))
                    sb.Append($"{prefix}exp=3-5");

                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.FiveYearsAndAbove))
                    sb.Append($"{prefix}exp=5plus");

                this.hasQueryParams = true;
            }
        }

        private void AddGradePath(StringBuilder sb, Grades? grades)
        {
            if (grades != null)
            {
                string prefix = string.Empty;

                if (this.hasSearch && this.hasQueryParams)
                {
                    prefix = "+";
                }

                if (!this.hasSearch && this.hasQueryParams)
                {
                    prefix = "&search=";
                }

                if (!this.hasSearch && !this.hasQueryParams)
                {
                    prefix = "?search=";
                }

                if (((Grades)grades).HasFlag(Grades.TraineeIntern))
                    sb.Append($"{prefix}Trainee");

                if (((Grades)grades).HasFlag(Grades.Junior))
                    sb.Append($"{prefix}Junior");

                if (((Grades)grades).HasFlag(Grades.Middle))
                    sb.Append($"{prefix}Middle");

                if (((Grades)grades).HasFlag(Grades.Senior))
                    sb.Append($"{prefix}Senior");

                if (((Grades)grades).HasFlag(Grades.TeamLead))
                    sb.Append($"{prefix}Team Lead");

                if (((Grades)grades).HasFlag(Grades.HeadChief))
                    sb.Append($"{prefix}Chief Head");
            }
        }
    }
}

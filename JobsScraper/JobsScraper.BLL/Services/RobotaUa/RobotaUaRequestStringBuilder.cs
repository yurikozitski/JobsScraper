using System.Text;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Extensions;
using JobsScraper.BLL.Interfaces.RobotaUa;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.BLL.Services.RobotaUa
{
    public class RobotaUaRequestStringBuilder : IRobotaUaRequestStringBuilder
    {
        private static bool hasQueryParams;
        private readonly IConfiguration configuration;

        public string RequestString { get; private set; } = default!;

        public RobotaUaRequestStringBuilder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetRequestString(JobSearchModel jobSearchModel)
        {
            ArgumentNullException.ThrowIfNull(jobSearchModel);

            StringBuilder requestStringBuilder = new StringBuilder(this.configuration["RobotaUa:Domain"]);

            AddJobStackPath(requestStringBuilder, jobSearchModel.JobStack);
            AddGradePath(requestStringBuilder, jobSearchModel.Grade);
            AddLocationPath(requestStringBuilder, jobSearchModel.Country, jobSearchModel.City);
            AddJobTypesPath(requestStringBuilder, jobSearchModel.JobType);
            AddSalaryPath(requestStringBuilder, jobSearchModel.SalaryFrom);
            AddExperienceLevelPath(requestStringBuilder, jobSearchModel.ExperienceLevel);

            string requestString = requestStringBuilder.ToString();
            this.RequestString = requestString;

            return requestString;
        }

        private static void AddJobStackPath(StringBuilder sb, JobStacks jobStacks)
        {
            sb.Append(jobStacks.ToQueryParam(JobBoards.RobotaUa));
        }

        private static void AddJobTypesPath(StringBuilder sb, JobTypes? jobTypes)
        {
            if (jobTypes != null)
            {
                if (((JobTypes)jobTypes).HasFlag(JobTypes.OnSite))
                    sb.Append("?scheduleIds=9");

                if (((JobTypes)jobTypes).HasFlag(JobTypes.Hybrid))
                    sb.Append("?scheduleIds=8");

                if (((JobTypes)jobTypes).HasFlag(JobTypes.Remote))
                    sb.Append("?scheduleIds=3");

                hasQueryParams = true;
            }
        }

        private static void AddLocationPath(StringBuilder sb, Countries? countries, Cities? cities)
        {
            if (cities != null)
            {
                if (((Cities)cities).HasFlag(Cities.Kyiv))
                    sb.Append("/kyiv");

                if (((Cities)cities).HasFlag(Cities.Vinnytsia))
                    sb.Append("/vinnytsia");

                if (((Cities)cities).HasFlag(Cities.Dnipro))
                    sb.Append("/dnipro");

                if (((Cities)cities).HasFlag(Cities.IvanoFrankivsk))
                    sb.Append("/ivanofrankivsk");

                if (((Cities)cities).HasFlag(Cities.Zhytomyr))
                    sb.Append("/zhytomyr");

                if (((Cities)cities).HasFlag(Cities.Zaporizhzhia))
                    sb.Append("/zaporizhzhia");

                if (((Cities)cities).HasFlag(Cities.Lviv))
                    sb.Append("/lviv");

                if (((Cities)cities).HasFlag(Cities.Mykolaiv))
                    sb.Append("/mykolaiv");

                if (((Cities)cities).HasFlag(Cities.Odesa))
                    sb.Append("/odesa");

                if (((Cities)cities).HasFlag(Cities.Ternopil))
                    sb.Append("/ternopil");

                if (((Cities)cities).HasFlag(Cities.Kharkiv))
                    sb.Append("/kharkiv");

                if (((Cities)cities).HasFlag(Cities.Khmelnytskyi))
                    sb.Append("/khmelnytskyi");

                if (((Cities)cities).HasFlag(Cities.Cherkasy))
                    sb.Append("/cherkasy");

                if (((Cities)cities).HasFlag(Cities.Chernihiv))
                    sb.Append("/chernihiv");

                if (((Cities)cities).HasFlag(Cities.Chernivtsi))
                    sb.Append("/chernivtsi");

                if (((Cities)cities).HasFlag(Cities.Uzhhorod))
                    sb.Append("/uzhhorod");

                return;
            }

            if (countries != null)
            {
                if (((Countries)countries).HasFlag(Countries.Ukraine))
                    sb.Append("/ukraine");

                if (((Countries)countries).HasFlag(Countries.Poland))
                    sb.Append("/other_countries");

                if (((Countries)countries).HasFlag(Countries.EU))
                    sb.Append("/other_countries");

                if (((Countries)countries).HasFlag(Countries.Other))
                    sb.Append("/other_countries");

                return;
            }

            sb.Append("/ukraine");
        }

        private static void AddExperienceLevelPath(StringBuilder sb, ExperienceLevels? experienceLevels)
        {
            if (experienceLevels != null)
            {
                if (((ExperienceLevels)experienceLevels).HasFlag(ExperienceLevels.NoExperience))
                {
                    if (hasQueryParams)
                        sb.Append("&");
                    else
                        sb.Append("?");

                    sb.Append("experienceType=true");
                }
            }
        }

        private static void AddGradePath(StringBuilder sb, Grades? grades)
        {
            if (grades != null)
            {
                sb.Append("-");

                if (((Grades)grades).HasFlag(Grades.TraineeIntern))
                    sb.Append("trainee");

                if (((Grades)grades).HasFlag(Grades.Junior))
                    sb.Append("junior");

                if (((Grades)grades).HasFlag(Grades.Middle))
                    sb.Append("middle");

                if (((Grades)grades).HasFlag(Grades.Senior))
                    sb.Append("senior");

                if (((Grades)grades).HasFlag(Grades.TeamLead))
                    sb.Append("team lead");

                if (((Grades)grades).HasFlag(Grades.HeadChief))
                    sb.Append("head");
            }
        }

        private static void AddSalaryPath(StringBuilder sb, int? salary)
        {
            if (salary != null)
            {
                if (hasQueryParams)
                    sb.Append("&");
                else
                    sb.Append("?");

                sb.Append($"salary={salary * 40}");
            }
        }
    }
}

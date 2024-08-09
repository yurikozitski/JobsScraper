using System.Text;
using JobsScraper.BLL.Enums;
using JobsScraper.BLL.Extensions;
using JobsScraper.BLL.Interfaces.Recruitika;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;

namespace JobsScraper.BLL.Services.Recruitika
{
    public class RecruitikaRequestStringBuilder : IRecruitikaRequestStringBuilder
    {
        private readonly IConfiguration configuration;
        private bool hasCollections;
        private bool hasCities;

        public string RequestString { get; private set; } = default!;

        public RecruitikaRequestStringBuilder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetRequestString(JobSearchModel jobSearchModel)
        {
            ArgumentNullException.ThrowIfNull(jobSearchModel);

            StringBuilder requestStringBuilder = new StringBuilder(this.configuration["Recruitika:Domain"]);

            AddJobStackPath(requestStringBuilder, jobSearchModel.JobStack);
            this.AddJobTypesPath(requestStringBuilder, jobSearchModel.JobType);
            this.AddGradePath(requestStringBuilder, jobSearchModel.Grade);
            this.AddCityPath(requestStringBuilder, jobSearchModel.City);

            string requestString = requestStringBuilder.ToString();
            this.RequestString = requestString;

            return requestString;
        }

        private static void AddJobStackPath(StringBuilder sb, JobStacks jobStacks)
        {
            sb.Append("?tags=");
            sb.Append(jobStacks.ToQueryParam(JobBoards.Recruitika));
        }

        private void AddJobTypesPath(StringBuilder sb, JobTypes? jobTypes)
        {
            if (jobTypes != null)
            {
                sb.Append("&collections=");

                if (((JobTypes)jobTypes).HasFlag(JobTypes.Hybrid))
                {
                    sb.Append("part-time");
                    this.hasCollections = true;
                }

                if (((JobTypes)jobTypes).HasFlag(JobTypes.Remote))
                {
                    if (this.hasCollections)
                        sb.Append(",");

                    sb.Append("remote");
                    this.hasCollections = true;
                }
            }
        }

        private void AddCityPath(StringBuilder sb, Cities? cities)
        {
            if (cities != null)
            {
                sb.Append("&cities=");

                if (((Cities)cities).HasFlag(Cities.Kyiv))
                {
                    sb.Append("kyiv");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Vinnytsia))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("vinnytsia");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Dnipro))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("dnepr");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.IvanoFrankivsk))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("ivano-frankovsk");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Zhytomyr))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("zhitomir");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Zaporizhzhia))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("zaporizhia");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Lviv))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("lvov");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Mykolaiv))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("mykolaiv");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Odesa))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("odessa");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Ternopil))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("ternopil");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Kharkiv))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("harkiv");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Khmelnytskyi))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("hmelnitskii");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Cherkasy))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("cherkasy");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Chernihiv))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("chernihiv");
                    this.hasCities = true;
                }

                if (((Cities)cities).HasFlag(Cities.Chernivtsi))
                {
                    if (this.hasCities)
                        sb.Append(",");

                    sb.Append("chernivtsi");
                    this.hasCities = true;
                }
            }
        }

        private void AddGradePath(StringBuilder sb, Grades? grades)
        {
            if (grades != null)
            {
                if (((Grades)grades).HasFlag(Grades.TraineeIntern) || ((Grades)grades).HasFlag(Grades.Junior))
                {
                    if (this.hasCollections)
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        sb.Append("&collections=");
                    }

                    sb.Append("juniorfriendly");
                    this.hasCollections = true;
                }
            }
        }
    }
}

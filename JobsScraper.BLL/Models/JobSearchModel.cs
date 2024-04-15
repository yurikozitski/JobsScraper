using JobsScraper.BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Models
{
    public class JobSearchModel
    {
        public JobStacks JobStack { get; set; }

        public JobTypes? JobType { get; set; }

        public Countries? Country { get; set; } 

        public Cities? City { get; set; } 

        public ExperienceLevels? ExperienceLevel { get; set; }

        public Grades? Grade { get; set; }

        public int? SalaryFrom { get; set; }

        public EnglishLevels? EnglishLevel { get; set; }
    }
}

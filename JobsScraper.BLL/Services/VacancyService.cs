using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Services
{
    public class VacancyService : IVacancyService
    {
        public VacancyService(IEnumerable<IJobBoardVacancyService> jobBoardVacancyServices) 
        { 

        }

        public IEnumerable<Vacancy> GetVacancies(JobSearchModel jobSearchModel) 
        {
            //check number of pages for each site

            //get djinni vacancies
            //get dou vacancies
            //get linkedin vacancies
        }
    }
}

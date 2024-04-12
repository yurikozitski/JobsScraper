using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Services
{
    public class DjinniVacancyService : IJobBoardVacancyService
    {
        private readonly IDjinniRequestStringBuilder djinniRequestStringBuilder;

        public DjinniVacancyService(IDjinniRequestStringBuilder djinniRequestStringBuilder)
        {
            this.djinniRequestStringBuilder = djinniRequestStringBuilder;
        }

        public IEnumerable<Task<string>> GetJobBoardVacancies(JobSearchModel jobSearchModel, int pagesNumber)
        {
            //get request string
            //make a request
        }
    }
}

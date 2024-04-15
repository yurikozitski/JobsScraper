using JobsScraper.BLL.Exceptions;
using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IDjinniHtmlLoader djinniHtmlLoader;
        private readonly IDjinniHtmlParser djinniHtmlParser;

        public VacancyService(IDjinniHtmlLoader djinniHtmlLoader, IDjinniHtmlParser djinniHtmlParser)
        {
            this.djinniHtmlLoader = djinniHtmlLoader;
            this.djinniHtmlParser = djinniHtmlParser;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(JobSearchModel jobSearchModel)
        {
            List<Vacancy> vacancies = new ();

            var djinniHtmlLoadTask = this.djinniHtmlLoader.LoadJobBoardHTMLAsync(jobSearchModel);

            await Task.WhenAll(djinniHtmlLoadTask);

            var djinniHtmlPasreTask = this.djinniHtmlParser.ParseJobBoardHTMLAsync(djinniHtmlLoadTask.Result!);

            await Task.WhenAll(djinniHtmlPasreTask);
            
            vacancies.AddRange(djinniHtmlPasreTask.Result);

            return vacancies;
        }
    }
}

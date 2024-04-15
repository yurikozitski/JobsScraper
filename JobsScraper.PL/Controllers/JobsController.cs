using JobsScraper.BLL.Exceptions;
using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobsScraper.PL.Controllers
{
    [Route("jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IVacancyService vacancyService;

        public JobsController(IVacancyService vacancyService)
        {
            this.vacancyService = vacancyService;
        }

        [HttpGet("find")]
        public async Task<ActionResult<IEnumerable<Vacancy>>> GetJobs([FromQuery]JobSearchModel jobSearchModel)
        {
            var vacancies = await this.vacancyService.GetVacanciesAsync(jobSearchModel);
            return Ok(vacancies);
        }
    }
}

using JobsScraper.BLL.Exceptions;
using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
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
        public async Task<ActionResult<IEnumerable<Vacancy>>> GetJobs([FromQuery]JobSearchModel jobSearchModel, CancellationToken token)
        {
            IEnumerable<Vacancy> vacancies = new List<Vacancy>();

            try
            {
                vacancies = await this.vacancyService.GetVacanciesAsync(jobSearchModel, token);
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested)
            {
                return StatusCode(504);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }

            return Ok(vacancies);
        }
    }
}

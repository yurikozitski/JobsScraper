using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Models;
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
        public async Task<ActionResult<IEnumerable<Vacancy>>> GetJobs([FromQuery] JobSearchModel jobSearchModel, CancellationToken token)
        {
            try
            {
                var vacancies = await this.vacancyService.GetVacanciesAsync(jobSearchModel, token);
                return Ok(vacancies);
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested)
            {
                return StatusCode(504);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }
    }
}

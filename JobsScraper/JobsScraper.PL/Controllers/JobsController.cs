using FluentValidation;
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
        private readonly IValidator<JobSearchModel> validator;

        public JobsController(IVacancyService vacancyService, IValidator<JobSearchModel> validator)
        {
            this.vacancyService = vacancyService;
            this.validator = validator;
        }

        [HttpGet("find")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
        public async Task<ActionResult<IEnumerable<Vacancy>>> GetJobs([FromQuery] JobSearchModel jobSearchModel, CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(jobSearchModel);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

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

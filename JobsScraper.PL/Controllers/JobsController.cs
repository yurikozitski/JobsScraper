using JobsScraper.BLL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobsScraper.PL.Controllers
{
    [Route("jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        public JobsController()
        {
            
        }

        [HttpGet("find")]
        public async Task<IActionResult> GetJobs([FromQuery]JobSearchModel jobSearchModel)
        {
            return Ok();
        }
    }
}

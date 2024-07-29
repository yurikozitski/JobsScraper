using FluentValidation;
using JobsScraper.BLL.Models;

namespace JobsScraper.BLL.Validation
{
    public class JobSearchModelValidator : AbstractValidator<JobSearchModel>
    {
        public JobSearchModelValidator()
        {
            this.RuleFor(x => x.JobStack).IsInEnum();
            this.RuleFor(x => x.JobType).IsInEnum();
            this.RuleFor(x => x.Grade).IsInEnum();
            this.RuleFor(x => x.ExperienceLevel).IsInEnum();
            this.RuleFor(x => x.Country).IsInEnum();
            this.RuleFor(x => x.City).IsInEnum();
            this.RuleFor(x => x.EnglishLevel).IsInEnum();
            this.RuleFor(x => x.SalaryFrom).InclusiveBetween(0, 10_000);
        }
    }
}

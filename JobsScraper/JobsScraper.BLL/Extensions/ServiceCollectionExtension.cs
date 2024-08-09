using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Interfaces.RobotaUa;
using JobsScraper.BLL.Services.Djinni;
using JobsScraper.BLL.Services.DOU;
using JobsScraper.BLL.Services.RobotaUa;
using Microsoft.Extensions.DependencyInjection;

namespace JobsScraper.BLL.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddDjinniServices(this IServiceCollection services)
        {
            services.AddScoped<IDjinniHtmlLoader, DjinniHtmlLoader>();
            services.AddScoped<IDjinniHtmlParser, DjinniHtmlParser>();
            services.AddScoped<IDjinniRequestStringBuilder, DjinniRequestStringBuilder>();
            services.AddScoped<IDjinniVacancyService, DjinniVacancyService>();
        }

        public static void AddDouServices(this IServiceCollection services)
        {
            services.AddScoped<IDouHtmlLoader, DouHtmlLoader>();
            services.AddScoped<IDouHtmlParser, DouHtmlParser>();
            services.AddScoped<IDouRequestStringBuilder, DouRequestStringBuilder>();
            services.AddScoped<IDouVacancyService, DouVacancyService>();
        }

        public static void AddRobotaUaServices(this IServiceCollection services)
        {
            services.AddScoped<IRobotaUaHtmlLoader, RobotaUaHtmlLoader>();
            services.AddScoped<IRobotaUaHtmlParser, RobotaUaHtmlParser>();
            services.AddScoped<IRobotaUaRequestStringBuilder, RobotaUaRequestStringBuilder>();
            services.AddScoped<IRobotaUaVacancyService, RobotaUaVacancyService>();
        }
    }
}

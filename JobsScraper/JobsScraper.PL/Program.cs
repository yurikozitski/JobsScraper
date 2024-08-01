using FluentValidation;
using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Interfaces.RobotaUa;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Services;
using JobsScraper.BLL.Services.Djinni;
using JobsScraper.BLL.Services.DOU;
using JobsScraper.BLL.Services.RobotaUa;
using JobsScraper.BLL.Validation;
using JobsScraper.PL.Middleware;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.AddScoped<IDjinniHtmlLoader, DjinniHtmlLoader>();
    builder.Services.AddScoped<IDjinniHtmlParser, DjinniHtmlParser>();
    builder.Services.AddScoped<IDjinniRequestStringBuilder, DjinniRequestStringBuilder>();
    builder.Services.AddScoped<IDjinniVacancyService, DjinniVacancyService>();

    builder.Services.AddScoped<IDouHtmlLoader, DouHtmlLoader>();
    builder.Services.AddScoped<IDouHtmlParser, DouHtmlParser>();
    builder.Services.AddScoped<IDouRequestStringBuilder, DouRequestStringBuilder>();
    builder.Services.AddScoped<IDouVacancyService, DouVacancyService>();

    builder.Services.AddScoped<IRobotaUaHtmlLoader, RobotaUaHtmlLoader>();
    builder.Services.AddScoped<IRobotaUaHtmlParser, RobotaUaHtmlParser>();
    builder.Services.AddScoped<IRobotaUaRequestStringBuilder, RobotaUaRequestStringBuilder>();
    builder.Services.AddScoped<IRobotaUaVacancyService, RobotaUaVacancyService>();

    builder.Services.AddScoped<IVacancyService, VacancyService>();

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IValidator<JobSearchModel>, JobSearchModelValidator>();
    ValidatorOptions.Global.LanguageManager.Enabled = false;

    builder.Configuration.AddJsonFile("parsingconfig.json");

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //builder.Services.Configure<ApiBehaviorOptions>(options =>
    //{
    //    options.SuppressModelStateInvalidFilter = true;
    //});
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ErrorHandlingMiddleware>();

    //app.UseHttpsRedirection();

    //app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
using FluentValidation;
using JobsScraper.BLL.Extensions;
using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Models;
using JobsScraper.BLL.Services;
using JobsScraper.BLL.Validation;
using JobsScraper.PL.Middleware;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddDjinniServices();
    builder.Services.AddDouServices();
    builder.Services.AddRobotaUaServices();

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
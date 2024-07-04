using JobsScraper.BLL.Interfaces;
using JobsScraper.BLL.Interfaces.Djinni;
using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Services;
using JobsScraper.BLL.Services.Djinni;
using JobsScraper.BLL.Services.DOU;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IDjinniHtmlLoader, DjinniHtmlLoader>();
builder.Services.AddScoped<IDjinniHtmlParser, DjinniHtmlParser>();
builder.Services.AddScoped<IDjinniRequestStringBuilder, DjinniRequestStringBuilder>();

builder.Services.AddScoped<IDouHtmlLoader, DouHtmlLoader>();
builder.Services.AddScoped<IDouHtmlParser, DouHtmlParser>();
builder.Services.AddScoped<IDouRequestStringBuilder, DouRequestStringBuilder>();

builder.Services.AddScoped<IVacancyService, VacancyService>();

builder.Services.AddHttpClient();

builder.Configuration.AddJsonFile("parsingconfig.json");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using ExchangeRates.Application.Interfaces;
using ExchangeRates.Infrastructure.Options;
using ExchangeRates.Infrastructure.Parsing;
using ExchangeRates.Infrastructure.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CnbOptions>(builder.Configuration.GetSection(CnbOptions.SectionName));
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CnbDailyRatesParser>();
builder.Services.AddHttpClient<IExchangeRateProvider, CnbExchangeRateProvider>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendDev");

app.MapControllers();

app.Run();

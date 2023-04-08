using System.Reflection;
using Application;
using Microsoft.AspNetCore.Mvc.Versioning;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Persistence;
using Serilog;
using Shared;
using API.Extensions;
using Application.Models;
using Microsoft.OpenApi.Models;
using Persistence.Implementation.Audit;
using Hangfire;
using API.Exceptions;
using Application.Contracts.Infrastructure;
using AspNetCoreRateLimit;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http.Features;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var indexPerMonth = false;
var amountOfPreviousIndicesUsedInAlias = 3;

var builder = WebApplication.CreateBuilder(args);

// serilog configuration added
builder.Host.UseSerilog(SeriLogger.Configure);

builder.WebHost.ConfigureKestrel(ck =>
{
    ck.ConfigureHttpsDefaults(httpDf =>
    {
        httpDf.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
    });
});

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Pagination");
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

#region -- Configures services for Multipart body length
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});
#endregion

#region-- Hangfire Setup
builder.Services.AddHangfire(x =>
{
    x.UseSerializerSettings(new Newtonsoft.Json.JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All });
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DronesAppConnectionString"));
});
builder.Services.AddHangfireServer();
#endregion

builder.Services.Configure<ElasticSearchSettings>(builder.Configuration.GetSection("ElasticConfiguration"));
// builder.Services.Configure<DroneConfiguration>(builder.Configuration.GetSection("DroneConfiguration"));
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddAuditTrail<AuditTrailLog>(options =>
    options.UseSettings(indexPerMonth, amountOfPreviousIndicesUsedInAlias));

builder.Services.AddMemoryCache();

// configure rate limiting
builder.Services.ConfigureRateLmitOptions(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region -- Swagger Support and API versioning
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Drone App API",
        Description = "A Musala Soft Test"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver"));
});

builder.Services.AddVersionedApiExplorer(options => 
{ 
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region -- Health Check Support
builder.Services.AddHealthChecks().AddDbContextCheck<DronesAppContext>(); // add health checks for db
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseMiddleware<GlobalErrorHandlerMiddleware>();

app.UseIpRateLimiting();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

var options = new DashboardOptions()
{
    Authorization = new[] { new MyAuthorizationFilter() }
};
app.UseHangfireDashboard("/hangfire", options);

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

});

RecurringJob.AddOrUpdate<IDroneBatteryCheckerService>("Drone Battery Level Checker", d => d.CheckDroneBatteryLevels(), 
    builder.Configuration.GetSection("DroneBatteryLevelInterval").Value);

app.MigrateDatabase<DronesAppContext>((context, services) =>
{
    var logger = services.GetService<ILogger<DronesAppContextSeeder>>();
    DronesAppContextSeeder
        .SeedAsync(context, logger)
        .Wait();

}).Run();

// to enable hangfire dashboard in docker
public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{ 
    public bool Authorize(DashboardContext context) => true;
}
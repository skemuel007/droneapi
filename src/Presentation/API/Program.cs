using System.Reflection;
using Application;
using Microsoft.AspNetCore.Mvc.Versioning;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using Serilog;
using Shared;
using API.Extensions;
using Application.Models;
using Microsoft.OpenApi.Models;
using Persistence.Implementation.Audit;

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

builder.Services.Configure<ElasticSearchSettings>(builder.Configuration.GetSection("ElasticConfiguration"));
// builder.Services.Configure<DroneConfiguration>(builder.Configuration.GetSection("DroneConfiguration"));
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddAuditTrail<AuditTrailLog>(options =>
    options.UseSettings(indexPerMonth, amountOfPreviousIndicesUsedInAlias));

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

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

});

app.MigrateDatabase<DronesAppContext>((context, services) =>
{
    var logger = services.GetService<ILogger<DronesAppContextSeeder>>();
    DronesAppContextSeeder
        .SeedAsync(context, logger)
        .Wait();

}).Run();
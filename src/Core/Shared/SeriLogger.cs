using System;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using Serilog.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Shared
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, configuration) =>
        {

            var eleasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            configuration
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(eleasticUri))
            {
                IndexFormat = $"applogs-{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
                AutoRegisterTemplate = true,
                OverwriteTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                TypeName = null,
                BatchAction = ElasticOpType.Create,
                TemplateName = "DroneApi",
                NumberOfReplicas = 1,
                NumberOfShards = 2
            })
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
            .ReadFrom.Configuration(context.Configuration);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        };
    }
}


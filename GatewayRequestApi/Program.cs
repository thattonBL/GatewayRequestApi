using MediatR;
using GatewayRequestApi.Application.Behaviours;
using Message.Infrastructure;
using Microsoft.EntityFrameworkCore;
using GatewayRequestApi.Application.IntegrationEvents;
using Services.Common;
using IntegrationEventLogEF.Services;
using System.Data.Common;
using Message.Infrastructure.Repositories;
using Serilog;
using GatewayRequestApi.Queries;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace GatewayRequestApi
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Adds the Event Bus required for integration events
            builder.AddServiceDefaults();

            var appInsightsConnectionString = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")) ? builder.Configuration.GetConnectionString("ApplicationInsightConnectionString") : Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

            // Configure application insight logging
            builder.Logging.AddApplicationInsights(
                    configureTelemetryConfiguration: (config) =>
                    config.ConnectionString = appInsightsConnectionString,
                    configureApplicationInsightsLoggerOptions: (options) => { }
                );

            builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("gatewayRequestAPI", LogLevel.Trace);

            // Add services to the container.
            var connectionString = Environment.GetEnvironmentVariable("SQL_DB_CONNECTION_STRING");
            if (String.IsNullOrEmpty(connectionString))
            {
                connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            }

            builder.Services.AddDbContext<MessageContext>(options => options.UseSqlServer(connectionString));
            //builder.Services.AddDbContext<MessageContext>(options =>
            //{
            //    options.UseSqlServer(connectionString,
            //        sqlServerOptionsAction: sqlOptions =>
            //        {
            //            sqlOptions.EnableRetryOnFailure(
            //                            maxRetryCount: 5,
            //                            maxRetryDelay: TimeSpan.FromSeconds(30),
            //                            errorNumbersToAdd: null);
            //        });
            //});


            builder.Services.AddScoped<IMessageQueries>(sp => new MessageQueries(constr: connectionString));
            builder.Services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(sp => (DbConnection c) => new IntegrationEventLogService(c));

            builder.Services.AddTransient<IMessageIntegrationEventService, MessageIntegrationEventService>();

            var services = builder.Services;

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

                cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehaviour<,>));
            });

            services.AddScoped<IMessageRepository, MessageRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthorization();

            //app.UseSerilogRequestLogging();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MessageContext>();
                //var env = app.Services.GetService<IWebHostEnvironment>();
                //var settings = app.Services.GetService<IOptions<OrderingSettings>>();
                //var logger = app.Services.GetService<ILogger<OrderingContextSeed>>();
                //await context.Database.MigrateAsync();

                //await new OrderingContextSeed().SeedAsync(context, env, settings, logger);
                //var integEventContext = scope.ServiceProvider.GetRequiredService<IntegrationEventLogContext>();
                //await integEventContext.Database.MigrateAsync();
            }

            app.MapControllers();
            app.Run();
        }
    }
}

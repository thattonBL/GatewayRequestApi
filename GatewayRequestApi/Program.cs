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
using Elastic.CommonSchema.Serilog;
using Elastic.Serilog.Sinks;
using Elastic.Ingest.Elasticsearch;

namespace GatewayRequestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            builder.Host.UseSerilog((context, configuration) =>
            {
                var httpAccessor = context.Configuration.Get<HttpContextAccessor>();
                configuration.ReadFrom.Configuration(context.Configuration)
                             .Enrich.WithEcsHttpContext(httpAccessor)
                             .Enrich.WithEnvironmentName()
                             .WriteTo.ElasticCloud(context.Configuration["ElasticCloud:CloudId"], context.Configuration["ElasticCloud:CloudUser"], context.Configuration["ElasticCloud:CloudPass"], opts =>
                             {
                                 opts.DataStream = new Elastic.Ingest.Elasticsearch.DataStreams.DataStreamName("gateway-request-api-new-logs");
                                 opts.BootstrapMethod = BootstrapMethod.Failure;
                             });               
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Adds the Event Bus required for integration events
            builder.AddServiceDefaults();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

            if (connectionString != null)
            {
                connectionString = connectionString.Replace("{#host}", dbHost).Replace("{#dbName}", dbName).Replace("{#dbPassword}", dbPassword);
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

            builder.Services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(sp => (DbConnection c) => new IntegrationEventLogService(c));

            builder.Services.AddTransient<IMessageIntegrationEventService, MessageIntegrationEventService>();

            var services = builder.Services;

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

                cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                //cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
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

            app.UseSerilogRequestLogging();

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

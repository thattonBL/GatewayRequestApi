using Events.Common;
using GatewayRequestApi.Queries;
using Message.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace GatewayRequestApi.FunctionalTests;

public class MessageScenarioBase : WebApplicationFactory<Program>
{
    private class MessagingApplication
    {
        private Mock<IMessageQueries> _mockMessageQueries;
        public TestServer CreateServer()
        {
            _mockMessageQueries = new Mock<IMessageQueries>();
            _mockMessageQueries.Setup(m => m.GetRsiMessageAsync(It.IsAny<string>())).Returns(Task.FromResult(new RsiMessageView()));

            var builder = new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Specify the path to your mock appsettings file
                    var mockAppSettingsPath = "appsettings.Messaging.json";

                    // Add the mock appsettings file as a configuration source
                    config.AddJsonFile(mockAppSettingsPath);
                })
                .ConfigureServices(services =>
                {

                })
                .ConfigureTestServices(services =>
                {
                    //services.AddSingleton<IMessageRepository>(_mockMessageRepo.Object);
                    services.AddScoped<IMessageQueries>(q => _mockMessageQueries.Object);
                });

            var testServer = new TestServer(builder);
            return testServer;
        }
    }

    public TestServer CreateServer()
    {
        var factory = new MessagingApplication();
        return factory.CreateServer();
    }

    public static class Get
    {
        //public static string Messages = "api/GatewayMessage/rsi";

        public static string GetRsiMessageAsync(string identifier)
        {
            return $"api/GatewayMessage/{identifier}";
        }
    }

    public static class Post
    {
        public static string PostRsiMessage(RsiPostItem message)
        {
            return $"api/GatewayMessage/rsi/{message}";
        }
    }

    public static class Put
    {
        public static string CancelMessage = "api/GatewayMessage/cancel";
        //public static string ShipOrder = "api/v1/orders/ship";
    }

    //private class AuthStartupFilter : IStartupFilter
    //{
    //    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    //    {
    //        return app =>
    //        {
    //            app.UseMiddleware<AutoAuthorizeMiddleware>();

    //            next(app);
    //        };
    //    }
    //}
}
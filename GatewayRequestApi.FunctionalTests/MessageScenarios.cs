using Events.Common;
using GatewayRequestApi.Queries;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GatewayRequestApi.FunctionalTests;

public class MessageScenarios
{
    [Fact]
    public async Task Get_message_by_identifier_returns_ok_status_code()
    {
        var _mockMessageQueries = new Mock<IMessageQueries>();
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                //builder.ConfigureServices(services =>
                //{

                //})
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMessageQueries>(q => _mockMessageQueries.Object);
                });
            });
        var client = application.CreateClient();
        var identifier = "ABC123";
        var response = await client.GetAsync($"api/GatewayMessage/rsi/{identifier}");
        var s = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
    }

    //[Fact]
    //public async Task Cannot_post_rsi_without_identifier_invalid()
    //{
    //    // Arrange
    //    using var server = CreateServer();
    //    var client = server.CreateClient();
    //    var rsiPostItem = new RsiPostItem
    //    {
    //        PeriodicalDate = "02-07-1976",
    //        PublicationDate = "02-07-1976",
    //        ReaderType = "1"
    //    };

    //    // Act
    //    var response = await client.PostAsJsonAsync("/api/GatewayMessage/rsi", rsiPostItem);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //}
}

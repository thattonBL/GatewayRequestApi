using BL.Gateway.EventBus.Events;
using GatewayRequestApi.Application.IntegrationEvents;
using GatewayRequestApi.Application.IntegrationEvents.Events;
using GatewayRequestApi.Queries;
using Message.Domain.MessageAggregate;
using Message.Infrastructure;
using Message.Infrastructure.Repositories;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GatewayRequestApi.FunctionalTests;

public class MessageScenarios : IClassFixture<FunctionalTestWebAppFactory>
{
    private readonly FunctionalTestWebAppFactory _factory;

    public MessageScenarios(FunctionalTestWebAppFactory factory)
    {
        //
        _factory = factory;
    }
    
    //This was just a basic test to ensure that everything was wired up. Just put a simple Get ith no params on the Controller
    //[Fact]
    //public async Task Get_message_returns_ok_status_code()
    //{
    //    var _mockMessageQueries = new Mock<IMessageQueries>();
    //    var client = _factory.CreateClient();
    //    var response = await client.GetAsync($"api/GatewayMessage");
    //    var s = await response.Content.ReadAsStringAsync();
    //    response.EnsureSuccessStatusCode();
    //}

    [Fact]
    public async Task Get_message_by_identifier_returns_ok_status_code()
    {
        //Arrange
        var _mockMessageQueries = new Mock<IMessageQueries>();
        _mockMessageQueries.Setup(m => m.GetRsiMessageAsync(It.IsAny<string>())).Returns(Task.FromResult(new RsiMessageView()));
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IMessageQueries>(q => _mockMessageQueries.Object);
            });
        }).CreateClient();
        var identifier = "ABC123";

        //Act
        var response = await client.GetAsync($"api/GatewayMessage/rsi?identifier={identifier}");
        var s = await response.Content.ReadAsStringAsync();

        //Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_message_by_unknown_identifier_returns_404_not_found()
    {
        //Arrange
        var _mockMessageQueries = new Mock<IMessageQueries>();
        _mockMessageQueries.Setup(m => m.GetRsiMessageAsync(It.IsAny<string>())).Throws<Exception>();
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IMessageQueries>(q => _mockMessageQueries.Object);
            });
        }).CreateClient();
        var identifier = "ABC123";

        //Act
        var response = await client.GetAsync($"api/GatewayMessage/rsi?identifier={identifier}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Can_submit_rsi_success()
    {
        // Arrange       
        var content = new StringContent(SerializeCommandMessage(), UTF8Encoding.UTF8, "application/json");

        var _mockMsgIntegrationEventService = new Mock<IMessageIntegrationEventService>();
        _mockMsgIntegrationEventService.Setup(m => m.AddAndSaveEventAsync(It.IsAny<IntegrationEvent>())).Returns(Task.FromResult(1));

        var _mockMsgRepo = new Mock<IMessageRepository>();
        _mockMsgRepo.Setup(mk => mk.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var client = _factory.WithWebHostBuilder(builder =>
        {            
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<MessageContext>));
                services.AddDbContext<MessageContext>(options => options.UseSqlServer(_factory.DbConnectionString));

                services.AddScoped<IMessageRepository>(q => _mockMsgRepo.Object);
                services.AddTransient<IMessageIntegrationEventService>(i => _mockMsgIntegrationEventService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.PostAsync("api/GatewayMessage/rsi", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockMsgIntegrationEventService.Verify(m => m.AddAndSaveEventAsync(It.IsAny<NewRsiMessageSubmittedIntegrationEvent>()), Times.Once);
        _mockMsgIntegrationEventService.Verify(m => m.PublishEventsThroughEventBusAsync(It.IsAny<Guid>()), Times.Once);
        _mockMsgRepo.Verify(r => r.Add(It.IsAny<RsiMessage>()), Times.Once);
        _mockMsgRepo.Verify(r => r.AddCommon(MessageType.RSI, It.IsAny<int>()), Times.Once);
        _mockMsgRepo.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMsgRepo.Verify(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Return_Int_Server_Err_when_db_is_offline()
    {
        // Arrange       
        var content = new StringContent(SerializeCommandMessage(), UTF8Encoding.UTF8, "application/json");

        var _mockMsgIntegrationEventService = new Mock<IMessageIntegrationEventService>();
        _mockMsgIntegrationEventService.Setup(m => m.AddAndSaveEventAsync(It.IsAny<IntegrationEvent>())).Returns(Task.FromResult(1));

        var _mockMsgRepo = new Mock<IMessageRepository>();
        _mockMsgRepo.Setup(mk => mk.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<MessageContext>));
                services.AddDbContext<MessageContext>(options => options.UseSqlServer("no_sql_server_available"));

                services.AddScoped<IMessageRepository>(q => _mockMsgRepo.Object);
                services.AddTransient<IMessageIntegrationEventService>(i => _mockMsgIntegrationEventService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.PostAsync("api/GatewayMessage/rsi", content);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        _mockMsgIntegrationEventService.Verify(m => m.AddAndSaveEventAsync(It.IsAny<NewRsiMessageSubmittedIntegrationEvent>()), Times.Never);
        _mockMsgIntegrationEventService.Verify(m => m.PublishEventsThroughEventBusAsync(It.IsAny<Guid>()), Times.Never);
        _mockMsgRepo.Verify(r => r.Add(It.IsAny<RsiMessage>()), Times.Never);
        _mockMsgRepo.Verify(r => r.AddCommon(MessageType.RSI, It.IsAny<int>()), Times.Never);
        _mockMsgRepo.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockMsgRepo.Verify(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Bad_Request_Returned_When_Validation_Fails()
    {
        // Arrange
        var commandMessage = new
        {
            message = new
            {
                collectionCode = "TST",
                shelfmark = "tstMark",
                volumeNumber = "123",
                storageLocationCode = "33",
                author = "Christopher James",
                title = "A History of Yesterday",
                publicationDate = "23-04-2024",
                periodicalDate = "23-04-2024",
                articleLine1 = "hello",
                articleLine2 = "buddy",
                catalogueRecordUrl = "http://some/catalog/url",
                furtherDetailsUrl = "http://further/deets",
                dtRequired = "23-04-2024",
                route = "homeward bound",
                readingRoomStaffArea = "true",
                seatNumber = "15",
                readingCategory = "fiction",
                //identifier = "GHJ456", Remove identifier to cause validation to fail
                readerName = "Herod Antipas",
                readerType = "1",
                operatorInformation = "Have a word",
                itemIdentity = "The life and times of a silly boy"
            }
        };
        var serialisedMsg = JsonSerializer.Serialize(commandMessage);
        var content = new StringContent(serialisedMsg, UTF8Encoding.UTF8, "application/json");

        var _mockMsgIntegrationEventService = new Mock<IMessageIntegrationEventService>();
        _mockMsgIntegrationEventService.Setup(m => m.AddAndSaveEventAsync(It.IsAny<IntegrationEvent>())).Returns(Task.FromResult(1));

        var _mockMsgRepo = new Mock<IMessageRepository>();
        _mockMsgRepo.Setup(mk => mk.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<MessageContext>));
                services.AddDbContext<MessageContext>(options => options.UseSqlServer(_factory.DbConnectionString));

                services.AddScoped<IMessageRepository>(q => _mockMsgRepo.Object);
                services.AddTransient<IMessageIntegrationEventService>(i => _mockMsgIntegrationEventService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.PostAsync("api/GatewayMessage/rsi", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _mockMsgIntegrationEventService.Verify(m => m.AddAndSaveEventAsync(It.IsAny<NewRsiMessageSubmittedIntegrationEvent>()), Times.Never);
        _mockMsgIntegrationEventService.Verify(m => m.PublishEventsThroughEventBusAsync(It.IsAny<Guid>()), Times.Never);
        _mockMsgRepo.Verify(r => r.Add(It.IsAny<RsiMessage>()), Times.Never);
        _mockMsgRepo.Verify(r => r.AddCommon(MessageType.RSI, It.IsAny<int>()), Times.Never);
        _mockMsgRepo.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockMsgRepo.Verify(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static string SerializeCommandMessage()
    {
        var commandMessage = new
        {
            message = new
            {
                collectionCode = "TST",
                shelfmark = "tstMark",
                volumeNumber = "123",
                storageLocationCode = "33",
                author = "Christopher James",
                title = "A History of Yesterday",
                publicationDate = "23-04-2024",
                periodicalDate = "23-04-2024",
                articleLine1 = "hello",
                articleLine2 = "buddy",
                catalogueRecordUrl = "http://some/catalog/url",
                furtherDetailsUrl = "http://further/deets",
                dtRequired = "23-04-2024",
                route = "homeward bound",
                readingRoomStaffArea = "true",
                seatNumber = "15",
                readingCategory = "fiction",
                identifier = "GHJ456",
                readerName = "Herod Antipas",
                readerType = "1",
                operatorInformation = "Have a word",
                itemIdentity = "The life and times of a silly boy"
            }
        };
        var serialisedMsg = JsonSerializer.Serialize(commandMessage);
        return serialisedMsg;
    }
}

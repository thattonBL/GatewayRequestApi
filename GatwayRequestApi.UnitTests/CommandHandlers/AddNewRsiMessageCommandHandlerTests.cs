using GatewayRequestApi.Application.IntegrationEvents;
using GatewayRequestApi.Application.Commands;
using MediatR;
using Message.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using BL.Gateway.Events.Common;
using GatewayRequestApi.Validators;
using Message.Domain.MessageAggregate;

namespace GatwayRequestApi.UnitTests.CommandHandlers;

public class AddNewRsiMessageCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IMessageIntegrationEventService> _messageIntegrationEventService;
    private readonly Mock<IMessageRepository> _messageRepository;
    private readonly Mock<ILogger<AddNewRsiMessageCommandHandler>> _logger;

    public AddNewRsiMessageCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _messageIntegrationEventService = new Mock<IMessageIntegrationEventService>();
        _messageRepository = new Mock<IMessageRepository>();
        _logger = new Mock<ILogger<AddNewRsiMessageCommandHandler>>();
    }

    [Fact]
    public async Task CanAddRsiAndCommonToDomain()
    {
        //Arrange
        var mockRsiPostItem = new RsiPostItem();
        var mockAddRsiCommand = new AddNewRsiMessageCommand(CreateTestRsiPostItem());

        _mediator.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), default))
            .Returns(Task.FromResult(true));

        _messageRepository.Setup(r => r.UnitOfWork.SaveChangesAsync(default)).Returns(Task.FromResult(1));

        //Act
        var addRsiCommandHandler = new AddNewRsiMessageCommandHandler(_mediator.Object, _messageIntegrationEventService.Object, _messageRepository.Object, _logger.Object);
        var result = await addRsiCommandHandler.Handle(mockAddRsiCommand, new CancellationToken());

        //Assert
        Assert.True(result);
        _messageRepository.Verify(m => m.Add(It.IsAny<RsiMessage>()), Times.Once);
        _messageRepository.Verify(m => m.AddCommon(MessageType.RSI, It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task ValidationThrowsErrorWhenIdentityIsMissing()
    {
        //Arrange
        var mockRsiPostItem = new RsiPostItem { PublicationDate = "23-04-2024", PeriodicalDate = "23-04-2024", ReaderType = "1" };
        var addRsiCommand = new AddNewRsiMessageCommand(mockRsiPostItem);
        var commandValidator = new AddNewRsiMessageCommandValidator(new Mock<ILogger<AddNewRsiMessageCommandValidator>>().Object);

        //Act
        var validationResult = await commandValidator.ValidateAsync(addRsiCommand);

        //Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains("Item Identity' must not be empty.", validationResult.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ValidationThrowsErrorWhenReaderTypeIsNotInt()
    {
        //Arrange
        var mockRsiPostItem = new RsiPostItem { PublicationDate = "23-04-2024", PeriodicalDate = "23-04-2024", ItemIdentity = "ABC123", ReaderType = "Dude"};
        var addRsiCommand = new AddNewRsiMessageCommand(mockRsiPostItem);
        var commandValidator = new AddNewRsiMessageCommandValidator(new Mock<ILogger<AddNewRsiMessageCommandValidator>>().Object);

        //Act
        var validationResult = await commandValidator.ValidateAsync(addRsiCommand);

        //Assert
        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public async Task ValidationThrowsErrorWhenPublicationDateIsNotFormatted()
    {
        //Arrange
        var mockRsiPostItem = new RsiPostItem { PublicationDate = "23/04/2024", PeriodicalDate = "23-04-2024", ItemIdentity = "ABC123", ReaderType = "Dude" };
        var addRsiCommand = new AddNewRsiMessageCommand(mockRsiPostItem);
        var commandValidator = new AddNewRsiMessageCommandValidator(new Mock<ILogger<AddNewRsiMessageCommandValidator>>().Object);

        //Act
        var validationResult = await commandValidator.ValidateAsync(addRsiCommand);

        //Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains("Date format must be dd-MM-yyyy", validationResult.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ValidationThrowsErrorWhenPeriodicalDateIsNotFormatted()
    {
        //Arrange
        var mockRsiPostItem = new RsiPostItem { PublicationDate = "23-04-2024", PeriodicalDate = "23/04/2024", ItemIdentity = "ABC123", ReaderType = "Dude" };
        var addRsiCommand = new AddNewRsiMessageCommand(mockRsiPostItem);
        var commandValidator = new AddNewRsiMessageCommandValidator(new Mock<ILogger<AddNewRsiMessageCommandValidator>>().Object);

        //Act
        var validationResult = await commandValidator.ValidateAsync(addRsiCommand);

        //Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains("Date format must be dd-MM-yyyy", validationResult.Errors[0].ErrorMessage);
    }

    private RsiPostItem CreateTestRsiPostItem()
    {
        return new RsiPostItem
        {
            PublicationDate = "23-04-2024",
            PeriodicalDate = "23-04-2024",
            ReaderType = "1",
            ItemIdentity = "ABC123"
        };
    }
}

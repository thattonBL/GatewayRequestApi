using Events.Common;
using GatewayRequestApi.Application.Commands;
using GatewayRequestApi.Application.IntegrationEvents;
using MediatR;
using Message.Domain.MessageAggregate;
using Message.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatwayRequestApi.UnitTests.CommandHandlers;

public class CancelMessageCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IMessageRepository> _messageRepository;
    private readonly Mock<ILogger<CancelMessageCommandHandler>> _logger;

    public CancelMessageCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _messageRepository = new Mock<IMessageRepository>();
        _logger = new Mock<ILogger<CancelMessageCommandHandler>>();
    }

    [Fact]
    public async Task CanCancelMessage()
    {
        //Arrange
        var mockRsiCancelItem = new RsiCancelRequest { Identifier = "ABC123" };
        var mockCancelMessageCommand = new CancelMessageCommand(mockRsiCancelItem);

        _mediator.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), default))
            .Returns(Task.FromResult(true));

        var mockCommon = new Mock<CommonMessage>();
        //mockCommon.Setup(mk => mk.SetCancelledStatus(It.IsAny<string>())).Returns(true);
        var testTuple = new Tuple<CommonMessage, string>(mockCommon.Object, "ABC123");
        _messageRepository.Setup(repo => repo.GetCommonAsync(It.IsAny<string>())).Returns(Task.FromResult(testTuple));

        _messageRepository.Setup(r => r.UnitOfWork.SaveEntitiesAsync(default)).Returns(Task.FromResult(true));

        //Act
        var cancelMessageCommandHandler = new CancelMessageCommandHandler(_mediator.Object, _messageRepository.Object, _logger.Object);
        var result = await cancelMessageCommandHandler.Handle(mockCancelMessageCommand, new CancellationToken());

        //Assert 
        mockCommon.Verify(m => m.SetCancelledStatus(It.IsAny<string>()), Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task CannotCancelMessageThatDoesntExist()
    {
        //Arrange
        var mockRsiCancelItem = new RsiCancelRequest { Identifier = "ABC123" };
        var mockCancelMessageCommand = new CancelMessageCommand(mockRsiCancelItem);

        _mediator.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), default))
            .Returns(Task.FromResult(true));

        _messageRepository.Setup(repo => repo.GetCommonAsync(It.IsAny<string>())).Returns(Task.FromResult<Tuple<CommonMessage, string>>(null));

        _messageRepository.Setup(r => r.UnitOfWork.SaveEntitiesAsync(default)).Returns(Task.FromResult(true));

        //Act
        var cancelMessageCommandHandler = new CancelMessageCommandHandler(_mediator.Object, _messageRepository.Object, _logger.Object);
        var result = await cancelMessageCommandHandler.Handle(mockCancelMessageCommand, new CancellationToken());

        //Assert
        Assert.False(result);
    }
}
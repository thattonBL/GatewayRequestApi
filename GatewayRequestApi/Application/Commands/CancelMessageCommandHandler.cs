using GatewayRequestApi.Application.IntegrationEvents;
using Message.Infrastructure.Repositories;

namespace GatewayRequestApi.Application.Commands;

public class CancelMessageCommandHandler : IRequestHandler<CancelMessageCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<CancelMessageCommandHandler> _logger;

    public CancelMessageCommandHandler(IMediator mediator, IMessageRepository messageRepository, ILogger<CancelMessageCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentException(nameof(mediator));
        _messageRepository = messageRepository ?? throw new ArgumentException(nameof(messageRepository));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
    }

    public async Task<bool> Handle(CancelMessageCommand request, CancellationToken cancellationToken)
    {
        var messageToUpdate = await _messageRepository.GetCommonAsync(request.CancelRequest.Identifier);
        if(messageToUpdate == null)
        {
            return false;
        }
        messageToUpdate.Item1.SetCancelledStatus(messageToUpdate.Item2);
        return await _messageRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

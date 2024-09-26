using GatewayRequestApi.Application.Commands;
using GatewayRequestApi.Application.IntegrationEvents;
using GatewayRequestApi.Application.IntegrationEvents.Events;
using Message.Domain.Enums;
using Message.Domain.Events;
using Message.Infrastructure.Repositories;

namespace GatewayRequestApi.Application.DomainEventHandlers;

public class RequestCancelledDomainEventHandler : INotificationHandler<RequestCancelledDomainEvent>
{
    private readonly IMessageIntegrationEventService _messageIntegrationEventService;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<RequestCancelledDomainEventHandler> _logger;

    public RequestCancelledDomainEventHandler(IMessageIntegrationEventService messageIntegrationEventService,
                          IMessageRepository messageRepository,
                                 ILogger<RequestCancelledDomainEventHandler> logger)
    {
        _messageIntegrationEventService = messageIntegrationEventService;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task Handle(RequestCancelledDomainEvent request, CancellationToken cancellationToken)
    {
        var integrationEvent = new RequestStatusChangedToCancelledIntegrationEvent(request.Common.Id, request.Identifier, MessageStatusEnum.Cancelled.ToString(), RequestStatusChangedToCancelledIntegrationEvent.EVENT_NAME);
        await _messageIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}

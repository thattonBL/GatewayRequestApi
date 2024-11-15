using Events.Common;

namespace GatewayRequestApi.Application.Commands
{
    public class CancelMessageCommand :IRequest<bool>
    {
        public RsiCancelRequest CancelRequest { get; private set; }

        public CancelMessageCommand(RsiCancelRequest cancelRequest)
        {
            CancelRequest = cancelRequest;
        }
    }
}

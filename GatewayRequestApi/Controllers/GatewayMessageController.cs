using GatewayRequestApi.Application.Commands;
using GatewayRequestApi.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GatewayRequestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatewayMessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMessageQueries _messageQueries;
        private readonly ILogger<GatewayMessageController> _logger;

        public GatewayMessageController(IMessageQueries messageQueries, IMediator mediator, ILogger<GatewayMessageController> logger)
        {
            _messageQueries = messageQueries ?? throw new ArgumentException(nameof(messageQueries));
            _mediator = mediator ?? throw new ArgumentException(nameof(mediator));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        [Route("rsi")]
        [HttpGet]
        [ProducesResponseType(typeof(RsiMessageView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RsiMessageView>> GetRsiMessageAsync(string identifier)
        {
            try
            {
                var message = await _messageQueries.GetRsiMessageAsync(identifier);
                return message;
            }
            catch
            {
                return NotFound();
            }
        }

        // POST api/<GatewayMessageController>
        [Route("rsi")]       
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> CreateRsiMessageFromInput([FromBody] AddNewRsiMessageCommand addRsiMessageCommand)
        {
            //TODO: Add logging
            return await _mediator.Send(addRsiMessageCommand);
        }

        [Route("cancel")]
        [HttpPost]
        public async Task<ActionResult<bool>> CancelMessage([FromBody] CancelMessageCommand cancelMessage)
        {
            return await _mediator.Send(cancelMessage);
        }

        [Route("rea")]
        [HttpPost]
        public async Task<ActionResult<bool>> CreateReaMessageFromInput([FromBody] AddNewReaMessageCommand addReaMessageCommand)
        {
            //TODO: Add logging
            return await _mediator.Send(addReaMessageCommand);
        }
    }
}

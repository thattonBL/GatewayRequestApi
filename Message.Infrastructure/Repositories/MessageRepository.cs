using Message.Domain.Enums;
using Message.Domain.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository    {
        private readonly MessageContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public MessageRepository(MessageContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public RsiMessage Add(RsiMessage message)
        {
            //need to add new Queue, Common and RSI message not just RSI
            return _context.RSIs.Add(message).Entity;
        }

        public ReaMessage Add(ReaMessage message)
        {
            //need to add new Queue, Common and REA message not just RSI
            return _context.REAs.Add(message).Entity;
        }

        public async Task<CommonMessage> AddCommon(MessageType messageType, int messageId)
        {
            // Retrieve the existing messageTypeLookup entity
            var messageTypeLookup = await GetMessageTypeLookupByIdAsync((int) messageType);

            if (messageTypeLookup == null)
            {
                throw new InvalidOperationException("The specified messageTypeLookup entity does not exist.");
            }

            var newCommon = new CommonMessage(MessageStatusEnum.Received.ToString(), "", messageId, "", (int) messageType, "", "", 0, DateTime.Now, messageTypeLookup);
            return _context.Common.Add(newCommon).Entity;
        }

        private async Task<messageTypeLookup> GetMessageTypeLookupByIdAsync(int id)
        {
            return await _context.messageTypeLookups.FindAsync(id);
        }

        public async Task<Tuple<CommonMessage, string>> GetCommonAsync(string msgId)
        {
            var common = await _context.RSIs.Where(order => order.Identifier == msgId)
                                            .Join(_context.Common, order => order.Id, common => common.msg_target, (order, common) => new { Order = order, Common = common })
                                            .Where(x => x.Common.m_type == (int)MessageType.RSI)
                                            .Select(x => x.Common)
                                            .FirstOrDefaultAsync();
            if (common == null)
            {
                common = _context.RSIs.Local.Where(order => order.Identifier == msgId)
                                            .Join(_context.Common, order => order.Id, common => common.msg_target, (order, common) => new { Order = order, Common = common })
                                            .Where(x => x.Common.m_type == (int)MessageType.RSI)
                                            .Select(x => x.Common)
                                            .FirstOrDefault();
            }
            return new(common, msgId);
        }
    }
}

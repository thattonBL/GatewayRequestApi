using Message.Domain.MessageAggregate;

namespace Message.Infrastructure.Repositories
{
    public interface IMessageRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task<RsiMessage> Add(RsiMessage message);

        ReaMessage Add(ReaMessage message);

        Task<CommonMessage> AddCommon(MessageType messageType, int messageId);

        Task<Tuple<CommonMessage, string>> GetCommonAsync(string msgId);
    }
}
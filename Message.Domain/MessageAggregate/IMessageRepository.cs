using Message.Domain.MessageAggregate;

namespace Message.Infrastructure.Repositories
{
    public interface IMessageRepository
    {
        IUnitOfWork UnitOfWork { get; }

        RsiMessage Add(RsiMessage message);

        ReaMessage Add(ReaMessage message);

        Task<CommonMessage> AddCommon(MessageType messageType, int messageId);

        Task<Tuple<CommonMessage, string>> GetCommonAsync(string msgId);
    }
}
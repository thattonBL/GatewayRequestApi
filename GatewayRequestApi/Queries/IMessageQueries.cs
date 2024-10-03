
namespace GatewayRequestApi.Queries
{
    public interface IMessageQueries
    {
        Task<RsiMessageView> GetRsiMessageAsync(string identifier);
    }
}
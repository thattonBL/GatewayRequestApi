using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GatewayRequestApi.Queries;

public class MessageQueries : IMessageQueries
{
    private string _connectionString = string.Empty;
    public MessageQueries(string constr)
    {
        _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
    }

    public async Task<RsiMessageView> GetRsiMessageAsync(string identifier)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT * FROM RSI WHERE Identifier = @Identifier";
            var parameters = new { Identifier = identifier };
            var result = await connection.QueryFirstOrDefaultAsync<RsiMessageView>(query, parameters);
            if (result == null)
            {
                throw new Exception("No identifier Found");
            }
            return result;
        }
    }
}

using Dapper;
using Microsoft.Extensions.Options;
using OT.Assessment.Models;
using System.Data.SqlClient;

namespace OT.Assessments.Modules.PlayerAccountRepository
{
    public class PlayerAccountRepository : IPlayerAccountRepository
    {
        private readonly string _connectionString;

        public PlayerAccountRepository(IOptions<DatabaseSettings> config)
        {
            _connectionString = config.Value.DatabaseConnection;
        }

        public async Task AddPlayerAccountAsync(PlayerAccount account)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "AddPlayerAccount";
                var parameters = new { account.AccountId, account.Username };
                await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<PlayerAccount> GetPlayerAccountByIdAsync(Guid accountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "GetPlayerAccountById";
                return await connection.QueryFirstOrDefaultAsync<PlayerAccount>(sql, new { AccountId = accountId }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }

}

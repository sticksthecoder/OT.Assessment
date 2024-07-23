using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;
using OT.Assessment.Models;
using Dapper;

namespace OT.Assessments.Modules.CasinoWagerRepository
{
    public class CasinoWagerRepository : ICasinoWagerRepository
    {
        private readonly string _connectionString;

        public CasinoWagerRepository(IOptions<DatabaseSettings> config)
        {
            _connectionString = config.Value.DatabaseConnection;
        }

        public async Task AddCasinoWagerAsync(CasinoWager wager)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "AddCasinoWager";
                var parameters = new
                {
                    wager.WagerId,
                    wager.GameName,
                    wager.Provider,
                    wager.AccountId,
                    wager.Amount,
                    wager.CreatedDateTime
                };
                await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}

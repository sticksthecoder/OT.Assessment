using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;
using OT.Assessment.Models;
using Dapper;
using OT.Assessment.Consumer.Settings;

namespace OT.Assessments.Modules.CasinoWagerRepository
{
    public class CasinoWagerRepository : ICasinoWagerRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<CasinoWagerRepository> _logger; 


        public CasinoWagerRepository(IOptions<DatabaseSettings> config, ILogger<CasinoWagerRepository> logger)
        {
            _connectionString = config.Value.DatabaseConnection;
            _logger = logger;
        }

        public async Task AddCasinoWagerAsync(CasinoWager wager)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Define the stored procedure name and parameters
                    var sql = "AddCasinoWager";
                    var parameters = new
                    {
                        wager.wagerId,
                        wager.gameName,
                        wager.provider,
                        wager.accountId,
                        wager.amount,
                        wager.createdDateTime
                    };

                    // Execute the stored procedure asynchronously
                    await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "A SQL error occurred while adding a casino wager: {WagerId}", wager.wagerId);
                throw new Exception("A database error occurred while adding the casino wager. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a casino wager: {WagerId}", wager.wagerId);
                throw new Exception("An error occurred while adding the casino wager. Please try again later.", ex);
            }
        }
    }
}

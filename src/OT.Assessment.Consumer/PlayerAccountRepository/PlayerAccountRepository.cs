using Dapper;
using Microsoft.Extensions.Options;
using OT.Assessment.Consumer.Settings;
using OT.Assessment.Models;
using System.Data.SqlClient;

namespace OT.Assessments.Modules.PlayerAccountRepository
{
    public class PlayerAccountRepository : IPlayerAccountRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<PlayerAccountRepository> _logger; 


        public PlayerAccountRepository(IOptions<DatabaseSettings> config, ILogger<PlayerAccountRepository> logger)
        {
            _connectionString = config.Value.DatabaseConnection;
            _logger = logger;
        }

        public async Task AddPlayerAccountAsync(PlayerAccount account)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Define the stored procedure name and parameters
                    var sql = "AddPlayerAccount";
                    var parameters = new { account.AccountId, account.Username };

                    await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {

                _logger.LogError(ex, "A SQL error occurred while adding a player account: {AccountId}", account.AccountId);
                throw new Exception("A database error occurred while adding the player account. Please try again later.", ex);
            }
            catch (Exception ex)
            {
              
                _logger.LogError(ex, "An error occurred while adding a player account: {AccountId}", account.AccountId);
                throw new Exception("An error occurred while adding the player account. Please try again later.", ex);
            }
        }

        // Method to get a player account by ID from the database
        public async Task<PlayerAccount> GetPlayerAccountByIdAsync(Guid accountId)
        {
            try
            {
         
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Define the stored procedure name and parameters
                    var sql = "GetPlayerAccountById";
                    var parameters = new { AccountId = accountId };

                    
                    return await connection.QueryFirstOrDefaultAsync<PlayerAccount>(sql, parameters, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                
                _logger.LogError(ex, "A SQL error occurred while retrieving a player account: {AccountId}", accountId);
                throw new Exception("A database error occurred while retrieving the player account. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "An error occurred while retrieving a player account: {AccountId}", accountId);
                throw new Exception("An error occurred while retrieving the player account. Please try again later.", ex);
            }
        }
    }

}

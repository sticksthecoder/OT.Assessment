using Dapper;
using Microsoft.Extensions.Options;
using OT.Assessment.App.Data;
using OT.Assessment.App.SQLConnection;
using System.Data.SqlClient;

namespace OT.Assessment.App.Repositories
{
public class CasinoWagerRepository : ICasinoWagerRepository
{
        private readonly ISqlDatabaseConnection _sqlDatabaseConnection;
        private readonly ILogger<CasinoWagerRepository> _logger;

        public CasinoWagerRepository(ISqlDatabaseConnection sqlDatabaseConnection, ILogger<CasinoWagerRepository> logger)
        {
            _sqlDatabaseConnection = sqlDatabaseConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<CasinoWagerDto>> GetWagersByPlayerIdAsync(Guid playerId, int page, int pageSize)
        {
            List<CasinoWagerDto> casinoWagers = new List<CasinoWagerDto>();

            try
            {
                using (var connection = _sqlDatabaseConnection.GetConnection())
                {
                    // SQL query to retrieve wagers with pagination
                    var sql = @"
                        SELECT WagerId, Game, Provider, Amount, CreatedDateTime AS CreatedDate
                        FROM PlayerCasinoWager
                        WHERE AccountId = @PlayerId
                        ORDER BY CreatedDateTime DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    // Execute the query and convert the results to a list
                    var result = (await connection.QueryAsync(sql, new
                    {
                        PlayerId = playerId,
                        Offset = (page - 1) * pageSize,
                        PageSize = pageSize
                    })).ToList();

                    // Loop through the results and add to the CasinoWagers list
                    foreach (var item in result)
                    {
                        var casinoWager = new CasinoWagerDto
                        {
                            Amount = item.Amount,
                            CreatedDate = item.CreatedDate,
                            Game = item.Game,
                            Provider = item.Provider,
                            WagerId = item.WagerId
                        };
                        casinoWagers.Add(casinoWager);
                    }

                }

                return casinoWagers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, @$"An error occurred while retrieving wagers for player ID {playerId}");
                throw new Exception("An error occurred while retrieving wagers. Please try again later.", ex);
            }
        }

        public async Task<int> GetTotalWagersByPlayerIdAsync(Guid playerId)
        {
            try
            {
                using (var connection = _sqlDatabaseConnection.GetConnection())
                {
                    // SQL query to count the total number of wagers for the player
                    var sql = "SELECT COUNT(*) FROM PlayerCasinoWager WHERE AccountId = @PlayerId";
                    return await connection.ExecuteScalarAsync<int>(sql, new { PlayerId = playerId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, @$"An error occurred while counting wagers for player ID {playerId}");
                throw new Exception("An error occurred while counting wagers. Please try again later.", ex);
            }
        }

        public async Task<IEnumerable<TopSpenderDto>> GetTopSpendersAsync(int count)
        {
            try
            {
                using (var connection = _sqlDatabaseConnection.GetConnection())
                {
                    // SQL query to retrieve the top spenders
                    var sql = @"
                        SELECT TOP (@Count) pcw.AccountId, pa.Username, SUM(pcw.Amount) AS TotalAmountSpend
                        FROM PlayerCasinoWager pcw 
                        INNER JOIN PlayerAccount pa 
                        ON pcw.AccountId = pa.AccountId
                        GROUP BY pcw.AccountId, Username
                        ORDER BY SUM(Amount) DESC";

                    return await connection.QueryAsync<TopSpenderDto>(sql, new { Count = count });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the top spenders");
                throw new Exception("An error occurred while retrieving the top spenders. Please try again later.", ex);
            }
        }
    }
}

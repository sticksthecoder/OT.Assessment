using OT.Assessment.App.Data;
using OT.Assessment.App.Repositories;

namespace OT.Assessment.App.Services
{
    public class CasinoWagerService : ICasinoWagerService
    {
        private readonly ICasinoWagerRepository _repository;

        private readonly ILogger<CasinoWagerService> _logger;

        public CasinoWagerService(ICasinoWagerRepository repository, ILogger<CasinoWagerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<TopSpenderDto>> GetTopSpendersAsync(int count)
        {
            try
            {
                // Call the repository method to get the top spenders
                return await _repository.GetTopSpendersAsync(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the top spenders");
                throw new Exception("An error occurred while retrieving the top spenders. Please try again later.", ex);
            }
        }

        public async Task<(IEnumerable<CasinoWagerDto> Wagers, int Total, int TotalPages)> GetWagersByPlayerIdAsync(Guid playerId, int page, int pageSize)
        {
            try
            {
                // Retrieve the list of wagers from the repository
                var wagers = (await _repository.GetWagersByPlayerIdAsync(playerId, page, pageSize)).ToList();

                // Get the total number of wagers for the player
                var total = await _repository.GetTotalWagersByPlayerIdAsync(playerId);

                // Calculate the total number of pages based on the page size
                var totalPages = (int)Math.Ceiling((double)total / pageSize);

                // Return the wagers, total number of wagers, and total pages
                return (wagers, total, totalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving wagers for player ID {PlayerId}", playerId);
                throw new Exception("An error occurred while retrieving wagers. Please try again later.", ex);
            }
        }
    }
}

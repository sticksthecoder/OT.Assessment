using OT.Assessment.App.Data;

namespace OT.Assessment.App.Services
{
    public interface ICasinoWagerService
    {
        Task<(IEnumerable<CasinoWagerDto> Wagers, int Total, int TotalPages)> GetWagersByPlayerIdAsync(Guid playerId, int page, int pageSize);
        Task<IEnumerable<TopSpenderDto>> GetTopSpendersAsync(int count);
    }
}

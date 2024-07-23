using OT.Assessment.App.Data;

namespace OT.Assessment.App.Repositories
{
    public interface ICasinoWagerRepository
    {
        Task<IEnumerable<CasinoWagerDto>> GetWagersByPlayerIdAsync(Guid playerId, int page, int pageSize);
        Task<int> GetTotalWagersByPlayerIdAsync(Guid playerId);
        Task<IEnumerable<TopSpenderDto>> GetTopSpendersAsync(int count);
    }
}

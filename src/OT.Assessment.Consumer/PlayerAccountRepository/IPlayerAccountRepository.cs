using OT.Assessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessments.Modules.PlayerAccountRepository
{
    public interface IPlayerAccountRepository
    {
        Task AddPlayerAccountAsync(PlayerAccount account);
        Task<PlayerAccount> GetPlayerAccountByIdAsync(Guid accountId);
    }

}

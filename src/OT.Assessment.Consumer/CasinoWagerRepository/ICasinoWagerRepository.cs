using OT.Assessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessments.Modules.CasinoWagerRepository
{
    public interface ICasinoWagerRepository
    {
        Task AddCasinoWagerAsync(CasinoWager wager);
    }
}

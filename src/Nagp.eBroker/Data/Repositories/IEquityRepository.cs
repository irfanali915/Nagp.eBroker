using Nagp.eBroker.Data.Entities;
using System.Threading.Tasks;

namespace Nagp.eBroker.Data.Repositories
{
    public interface IEquityRepository : IRepository<Equity>
    {
        Task<int> Save();
    }
}

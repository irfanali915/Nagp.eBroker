using Nagp.eBroker.Data.Entities;
using System.Threading.Tasks;

namespace Nagp.eBroker.Data.Repositories
{
    public interface IEquitieHoldRepository : IRepository<EquitieHold>
    {
        Task<int> Save();
    }
}

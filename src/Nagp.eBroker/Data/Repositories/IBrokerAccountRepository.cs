using Nagp.eBroker.Data.Entities;
using System.Threading.Tasks;

namespace Nagp.eBroker.Data.Repositories
{
    public interface IBrokerAccountRepository : IRepository<BrokerAccount>
    {
        Task<int> Save();
    }
}

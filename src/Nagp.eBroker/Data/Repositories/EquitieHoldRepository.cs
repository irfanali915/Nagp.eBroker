using Nagp.eBroker.Data.Entities;
using System.Threading.Tasks;

namespace Nagp.eBroker.Data.Repositories
{
    public class EquitieHoldRepository : GenericRepository<EquitieHold>, IEquitieHoldRepository
    {
        private readonly EBrokerContext _eBrokerContext;

        public EquitieHoldRepository(EBrokerContext eBrokerContext) : base(eBrokerContext)
        {
            _eBrokerContext = eBrokerContext;
        }

        public Task<int> Save()
        {
            return _eBrokerContext.SaveChangesAsync();
        }
    }
}

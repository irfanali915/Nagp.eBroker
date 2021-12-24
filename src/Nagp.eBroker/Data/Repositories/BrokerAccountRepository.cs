using Nagp.eBroker.Data.Entities;
using System.Threading.Tasks;

namespace Nagp.eBroker.Data.Repositories
{
    public class BrokerAccountRepository : GenericRepository<BrokerAccount>, IBrokerAccountRepository
    {
        private readonly EBrokerContext _eBrokerContext;

        public BrokerAccountRepository(EBrokerContext eBrokerContext) : base(eBrokerContext)
        {
            _eBrokerContext = eBrokerContext;
        }

        public Task<int> Save()
        {
            return _eBrokerContext.SaveChangesAsync();
        }
    }
}

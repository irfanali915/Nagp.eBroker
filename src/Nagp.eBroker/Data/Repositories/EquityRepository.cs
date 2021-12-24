using Nagp.eBroker.Data.Entities;
using System.Threading.Tasks;

namespace Nagp.eBroker.Data.Repositories
{
    public class EquityRepository : GenericRepository<Equity>, IEquityRepository
    {
        private readonly EBrokerContext _eBrokerContext;

        public EquityRepository(EBrokerContext eBrokerContext) : base(eBrokerContext)
        {
            _eBrokerContext = eBrokerContext;
        }

        public Task<int> Save()
         => _eBrokerContext.SaveChangesAsync();
    }
}

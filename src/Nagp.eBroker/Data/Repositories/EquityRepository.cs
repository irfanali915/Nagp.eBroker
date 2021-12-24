using Nagp.eBroker.Data.Entities;

namespace Nagp.eBroker.Data.Repositories
{
    public class EquityRepository : GenericRepository<Equity>, IEquityRepository
    {
        private readonly EBrokerContext _eBrokerContext;

        public EquityRepository(EBrokerContext eBrokerContext) : base(eBrokerContext)
        {
            _eBrokerContext = eBrokerContext;
        }

        public void Save()
        {
            _eBrokerContext.SaveChanges();
        }
    }
}

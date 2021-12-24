using Microsoft.Extensions.Logging;
using Nagp.eBroker.Data.Repositories;
using System.Threading.Tasks;

namespace Nagp.eBroker.Service
{
    public class BrokerService : IBrokerService
    {
        readonly ILogger _logger;
        readonly IBrokerAccountRepository _brokerAccountRepository;
        public BrokerService(ILogger<BrokerService> logger, IBrokerAccountRepository brokerAccountRepository)
        {
            _logger = logger;
            _brokerAccountRepository = brokerAccountRepository;
        }

        public async Task<bool> IsBrokerIdValid(int brokerId)
        {
            _logger.LogDebug("Checking if broker Id = {brokerId} is valid", brokerId);
            var brokeAccount = await _brokerAccountRepository.GetByID(brokerId);
            if (brokeAccount != null)
            {
                _logger.LogDebug("Found brokerId -{brokerId}, accountName - '{accountName}'", brokerId, brokeAccount.AccountName);
                return true;
            }
            _logger.LogInformation("Invalid brokerId - {brokerId}", brokerId);
            return false;
        }

        public async Task<bool> AddFound(int brokerId, decimal amount)
        {
            _logger.LogDebug("Adding found to broker account");
            var brokeAccount = await _brokerAccountRepository.GetByID(brokerId);
            if (brokeAccount == null)
            {
                _logger.LogDebug("Not found brokerId - {brokerId}, ", brokerId);
                return false;
            }

            decimal charges = amount > 100000 ? amount * .0005M : 0.0M;
            brokeAccount.Funds += amount - charges;
            _brokerAccountRepository.Update(brokeAccount);

            return (await _brokerAccountRepository.Save()) > 0;
        }
    }
}

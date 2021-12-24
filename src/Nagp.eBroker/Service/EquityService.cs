using Microsoft.Extensions.Logging;
using Nagp.eBroker.Data.Entities;
using Nagp.eBroker.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Nagp.eBroker.Service
{
    public class EquityService : IEquityService
    {
        readonly ILogger _logger;
        readonly IBrokerAccountRepository _brokerAccountRepository;
        readonly IEquityRepository _equityRepository;
        readonly IEquitieHoldRepository _equitieHoldRepository;
        public EquityService(ILogger<EquityService> logger, IBrokerAccountRepository brokerAccountRepository, IEquityRepository equityRepository, IEquitieHoldRepository equitieHoldRepository)
        {
            _logger = logger;
            _brokerAccountRepository = brokerAccountRepository;
            _equityRepository = equityRepository;
            _equitieHoldRepository = equitieHoldRepository;
        }

        public async Task<bool> BuyEquityUnits(int brokerId, int equityId, int numberOfUnitsToBuy)
        {
            _logger.LogDebug("BuyEquityUnits brokerId - {brokerId}, equityId - {equityId}, numberOfUnitsToBuy - {numberOfUnitsToBuy}", brokerId, equityId, numberOfUnitsToBuy);

            //Check if equityId valid
            var equity = await _equityRepository.GetByID(equityId);
            if (equity == null || equity.Units < numberOfUnitsToBuy)
            {
                _logger.LogInformation("Invalid or equity not available equityId - {equityId}", equityId);
                return false;
            }

            //Check if equity unit available and broker has sufficient funds
            var brokerAccount = await _brokerAccountRepository.GetByID(brokerId);
            if (brokerAccount.Funds < (equity.PricePerUnit * numberOfUnitsToBuy))
            {
                _logger.LogInformation("Insufficient fund, equityId - {equityId}", equityId);
                return false;
            }

            //Deduct fund from broker account
            brokerAccount.Funds -= (equity.PricePerUnit * numberOfUnitsToBuy);
            _brokerAccountRepository.Update(brokerAccount);

            //Update available units
            equity.Units -= numberOfUnitsToBuy;
            _equityRepository.Update(equity);

            var equityHold = await _equitieHoldRepository.GetByID(brokerId, equityId);
            if (equityHold == null) // Add hold equity units
            {
                await _equitieHoldRepository.Insert(new EquitieHold
                {
                    BrokerAccountId = brokerId,
                    EquityId = equityId,
                    HoldUnits = numberOfUnitsToBuy
                });
            }
            else
            {
                //Update hold units
                equityHold.HoldUnits += numberOfUnitsToBuy;
                _equitieHoldRepository.Update(equityHold);
            }
            return await _equitieHoldRepository.Save() > 0;
        }

        public async Task<bool> SellEquityUnits(int brokerId, int equityId, int numberOfUnitsToSell)
        {
            _logger.LogDebug("BuyEquityUnits brokerId - {brokerId}, equityId - {equityId}, numberOfUnitsToSell - {numberOfUnitsToSell}", brokerId, equityId, numberOfUnitsToSell);

            //Check if equity is on hold
            var equityHold = await _equitieHoldRepository.GetByID(brokerId, equityId);
            if (equityHold == null || equityHold.HoldUnits < numberOfUnitsToSell)
            {
                _logger.LogInformation("Invalid equity or Insufficient equity on hold, equityId - {equityId}", equityId);
                return false;
            }

            //Update hold units
            equityHold.HoldUnits -= numberOfUnitsToSell;
            _equitieHoldRepository.Update(equityHold);

            //Get equity
            var equity = await _equityRepository.GetByID(equityId);

            var sellAmount = equity.PricePerUnit * numberOfUnitsToSell;
            var brokerage = Math.Max(20, sellAmount * 0.0005M);

            //Add fund from broker account
            var brokerAccount = await _brokerAccountRepository.GetByID(brokerId);
            brokerAccount.Funds += (sellAmount - brokerage);
            _brokerAccountRepository.Update(brokerAccount);

            //Update available units
            equity.Units += numberOfUnitsToSell;
            _equityRepository.Update(equity);

            return await _equitieHoldRepository.Save() > 0;
        }
    }
}

using System.Threading.Tasks;

namespace Nagp.eBroker.Service
{
    public interface IEquityService
    {
        Task<bool> BuyEquityUnits(int brokerId, int equityId, int numberOfUnitsToBuy);
        Task<bool> SellEquityUnits(int brokerId, int equityId, int numberOfUnitsToSell);
    }
}

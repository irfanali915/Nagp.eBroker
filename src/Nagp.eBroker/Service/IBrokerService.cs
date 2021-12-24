using System.Threading.Tasks;

namespace Nagp.eBroker.Service
{
    public interface IBrokerService
    {
        Task<bool> IsBrokerIdValid(int brokerId);

        Task<bool> AddFound(int brokerId, decimal amount);
    }
}

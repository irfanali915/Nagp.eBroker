using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nagp.eBroker.Helper;
using Nagp.eBroker.Service;
using System;
using System.Threading.Tasks;

namespace Nagp.eBroker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrokerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBrokerService _brokerService;
        private readonly IEquityService _equityService;
        public BrokerController(ILogger<BrokerController> logger, IDateTimeProvider dateTimeProvider, IBrokerService brokerService, IEquityService equityService)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _equityService = equityService;
            _brokerService = brokerService;
        }

        [HttpGet]
        [Route("addfound/{brokerId}/{amount}")]
        public async Task<IActionResult> AddFound(int brokerId, decimal amount)
        {
            _logger.LogDebug("AddFound({brokerId}, {amount})", brokerId, amount);
            if (!await _brokerService.IsBrokerIdValid(brokerId))
            {
                return Forbid("Invalid BrokerId.");
            }
            return Ok(_brokerService.AddFound(brokerId, amount));
        }

        [HttpGet]
        [Route("buyequity/{brokerId}/{equityId}/{numberOfUnitsToBuy}")]
        public async Task<IActionResult> BuyEquityUnits(int brokerId, int equityId, int numberOfUnitsToBuy)
        {
            _logger.LogDebug("BuyEquityUnits({brokerId}, {equityId}, {numberOfUnitsToBuy})", brokerId, equityId, numberOfUnitsToBuy);
            if (!Utility.IsInDayRange(_dateTimeProvider.GetNow(), DayOfWeek.Monday, DayOfWeek.Friday) || !Utility.IsInTimeRange(_dateTimeProvider.GetNow(), 9, 15))
            {
                return BadRequest("Service closed at this time.");
            }

            if (!await _brokerService.IsBrokerIdValid(brokerId))
            {
                return Forbid("Invalid BrokerId.");
            }

            return Ok(_equityService.BuyEquityUnits(brokerId, equityId, numberOfUnitsToBuy));
        }

        [HttpGet]
        [Route("sellequity/{brokerId}/{equityId}/{numberOfUnitsToSell}")]
        public async Task<IActionResult> SellEquityUnits(int brokerId, int equityId, int numberOfUnitsToSell)
        {
            _logger.LogDebug("SellEquityUnits({brokerId}, {equityId}, {numberOfUnitsToSell})", brokerId, equityId, numberOfUnitsToSell);
            if (!Utility.IsInDayRange(_dateTimeProvider.GetNow(), DayOfWeek.Monday, DayOfWeek.Friday) || !Utility.IsInTimeRange(_dateTimeProvider.GetNow(), 9, 15))
            {
                return BadRequest("Service closed at this time.");
            }

            if (!await _brokerService.IsBrokerIdValid(brokerId))
            {
                return Forbid("Invalid BrokerId.");
            }

            return Ok(_equityService.SellEquityUnits(brokerId, equityId, numberOfUnitsToSell));
        }
    }
}

namespace Nagp.eBroker.Data.Entities
{
    public class EquitieHold
    {
        public int BrokerAccountId { get; set; }

        public int EquityId { get; set; }

        public int HoldUnits { get; set; }

        public BrokerAccount BrokerAccount { get; set; }

        public Equity Equity { get; set; }
    }
}

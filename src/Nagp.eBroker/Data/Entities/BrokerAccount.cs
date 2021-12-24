using System.Collections.Generic;

namespace Nagp.eBroker.Data.Entities
{
    public class BrokerAccount
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public decimal Funds { get; set; }

        public ICollection<EquitieHold> EquitieHolds { get; set; }
    }
}

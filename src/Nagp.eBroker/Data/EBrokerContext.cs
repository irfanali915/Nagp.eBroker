using Microsoft.EntityFrameworkCore;
using Nagp.eBroker.Data.Entities;

namespace Nagp.eBroker.Data
{
    public class EBrokerContext : DbContext
    {
        public DbSet<Equity> Equities { get; set; }

        public DbSet<BrokerAccount> BrokerAccounts { get; set; }

        public DbSet<EquitieHold> EquitieHolds { get; set; }

        public EBrokerContext(DbContextOptions<EBrokerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquitieHold>().HasKey(c => new { c.BrokerAccountId, c.EquityId });
        }
    }
}

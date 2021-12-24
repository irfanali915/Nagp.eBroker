using Microsoft.EntityFrameworkCore;
using Nagp.eBroker.Data;
using System;

namespace Nagp.eBroker.UnitTests
{
    public class MockContextHelper
    {
        private readonly DbContextOptions<EBrokerContext> optionsBuilder;

        private static readonly Lazy<MockContextHelper> instance = new(() => new());

        public static MockContextHelper Instance => instance.Value;

        private MockContextHelper()
        {

            optionsBuilder = new DbContextOptionsBuilder<EBrokerContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).EnableSensitiveDataLogging(true)
              .Options;
        }

        public EBrokerContext GetMockContextPostgres() => new(optionsBuilder);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nagp.eBroker.Data;
using Nagp.eBroker.Data.Repositories;
using Nagp.eBroker.Helper;
using Nagp.eBroker.Service;
using System.Diagnostics.CodeAnalysis;

namespace Nagp.eBroker
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EBrokerContext>(options => options.UseNpgsql(Configuration.GetConnectionString(nameof(EBrokerContext))));

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>()
                    .AddSingleton<IBrokerAccountRepository, BrokerAccountRepository>()
                    .AddSingleton<IEquitieHoldRepository, EquitieHoldRepository>()
                    .AddSingleton<IEquityRepository, EquityRepository>();

            services.AddTransient<IEquityService, EquityService>()
                    .AddTransient<IBrokerService, BrokerService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nagp.eBroker", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nagp.eBroker v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

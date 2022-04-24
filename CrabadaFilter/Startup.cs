using CrabadaFilter.AppSettings;
using CrabadaFilter.Configurations;
using CrabadaFilter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrabadaFilter {
    public class Startup
    {
        private readonly CrabadaSettings _crabadaSettings;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false);
            _crabadaSettings = CrabadaSettings.GetFromConfiguration(configuration);
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure your services here
            services.RegisterHttpClients(_crabadaSettings);
            services.AddSingleton<ICrabadaService, CrabadaService>();
        }
    }
}

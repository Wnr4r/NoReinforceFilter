using System;
using System.Net.Http.Headers;
using CrabadaFilter.AppSettings;
using CrabadaFilter.HttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace CrabadaFilter.Configurations {
    /// <summary>
    /// Configuration of HTTP Clients using IHttpClientFactory
    /// </summary>
    public static class HttpClientsConfiguration {

        /// <summary>
        /// Extension method to register http clients
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        public static void RegisterHttpClients(this IServiceCollection services, CrabadaSettings settings)
        {
            var productValue = new ProductInfoHeaderValue("Netcard", "1.0");

            services.AddHttpClient<ICrabadaClient, CrabadaClient>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.DefaultRequestHeaders.UserAgent.Add(productValue);
            });
        }
    }
}

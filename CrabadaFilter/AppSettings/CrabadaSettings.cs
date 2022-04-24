using Microsoft.Extensions.Configuration;

namespace CrabadaFilter.AppSettings
{
    public class CrabadaSettings
    {
        public string BaseUrl { get; set; }
        public string MineUrl { get; set; }
        public string LendingHistoryUrl { get; set; }
        public string CanJoinTeamUrl { get; set; }

        /// <summary>
        /// Get Crabada settings section from IConfiguration
        /// </summary>
        /// <param name="configuration">IConfiguration</param>
        /// <returns></returns>
        public static CrabadaSettings GetFromConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection(nameof(CrabadaSettings)).Get<CrabadaSettings>();
        }
    }
}

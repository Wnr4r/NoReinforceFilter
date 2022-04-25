using System.Threading.Tasks;
using CrabadaFilter.HttpClients;
using CrabadaFilter.Models;

namespace CrabadaFilter.Services {
    public class CrabadaService : ICrabadaService
    {
        private readonly ICrabadaClient _crabadaClient;

        public CrabadaService(ICrabadaClient crabadaClient)
        {
            _crabadaClient = crabadaClient;
        }

        public async Task<MineDto> GetMineDetailsAsync(int mineId) => await _crabadaClient.GetMineDetailsAsync(mineId);
       

        public async Task<CanJoinDto> GetCanJoinTeamInfoAsync(string address) => await _crabadaClient.GetCanJoinTeamInfoAsync(address);
       

        public async Task<LendingHistoryDto> GetLendingHistoryAsync(string address) => await _crabadaClient.GetLendingHistoryAsync(address);
        
    }
}

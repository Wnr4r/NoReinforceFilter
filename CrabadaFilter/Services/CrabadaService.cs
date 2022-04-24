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

        public async Task<MineDto> GetMineDetailsAsync(int mineId)
        {
            var mineDetails = await _crabadaClient.GetMineDetailsAsync(mineId);

            return mineDetails;
        }

        public async Task<CanJoinDto> GetCanJoinTeamInfoAsync(string address)
        {
            var canJoinTeamInfo = await _crabadaClient.GetCanJoinTeamInfoAsync(address);

            return canJoinTeamInfo;
        }

        public async Task<LendingHistoryDto> GetLendingHistoryAsync(string address)
        {
            var historyDetails = await _crabadaClient.GetLendingHistoryAsync(address);

            return historyDetails;
        }

    }
}

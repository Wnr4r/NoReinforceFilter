using System.Threading.Tasks;
using CrabadaFilter.Models;

namespace CrabadaFilter.Services {
    public class CrabadaService : ICrabadaService
    {
        private readonly ICrabadaClient _crabadaClient;

        public CrabadaService(ICrabadaClient crabadaClient)
        {
            _crabadaClient = crabadaClient;
        }

        public async Task<MineResponse> GetMineDetailsAsync(int mineId)
        {
            var mineDetails = await _crabadaClient.GetMineDetailsAsync(mineId);

            return mineDetails.Result;
        }

        public async Task<object> GetLendingHistoryAsync(string address)
        {
            var historyDetails = await _crabadaClient.GetLendingHistoryAsync(address);

            return historyDetails;
        }

        public async Task<object> GetCanJoinTeamInfoAsync(string address)
        {
            var canJoinTeamInfo = await _crabadaClient.GetCanJoinTeamInfoAsync(address);

            return canJoinTeamInfo;
        }
    }
}

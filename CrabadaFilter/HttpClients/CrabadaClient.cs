using CrabadaFilter.Common;
using CrabadaFilter.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrabadaFilter.HttpClients {
    public class CrabadaClient : ICrabadaClient {
        private readonly HttpClient _httpClient;

        public CrabadaClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MineDto> GetMineDetailsAsync(int mineId) {
            var response = await _httpClient.GetAsync($"mine/{mineId}");

            var content = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode) return new MineDto();


            content.TryDeserializeObject(out DataAnswer<MineDto> result);

            return result.Result;

        }

        public async Task<CanJoinDto> GetCanJoinTeamInfoAsync(string address)
        {
            var response = await _httpClient.GetAsync($"crabadas/can-join-team?user_address={address}");

            var content = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode) return new CanJoinDto();


            content.TryDeserializeObject(out DataAnswer<CanJoinDto> result);

            return result.Result;
        }

        public async Task<LendingHistoryDto> GetLendingHistoryAsync(string address)
        {
            var response = await _httpClient.GetAsync($"crabadas/lending/history?borrower_address={address}&orderBy=transaction_time&order=desc&limit=2");

            var content = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode) return new LendingHistoryDto();


            content.TryDeserializeObject(out DataAnswer<LendingHistoryDto> result);

            return result.Result;
        }
    }
}

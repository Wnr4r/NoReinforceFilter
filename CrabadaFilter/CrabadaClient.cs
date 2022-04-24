using System;
using System.Net.Http;
using System.Threading.Tasks;
using CrabadaFilter.Common;
using CrabadaFilter.Models;
using Newtonsoft.Json.Serialization;

namespace CrabadaFilter {
    public class CrabadaClient  : ICrabadaClient {
        private readonly HttpClient _httpClient;

        public CrabadaClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DataAnswer<MineResponse>> GetMineDetailsAsync(int mineId)
        {
            var response = await _httpClient.GetAsync($"mine/{mineId}");

            var content = await response.Content.ReadAsStringAsync();


            // if (!response.IsSuccessStatusCode) return DataAnswer<MineResponse>


            content.TryDeserializeObject(out DataAnswer<MineResponse> result);

            return result;

        }

        public async Task<object> GetLendingHistoryAsync(string address)
        {
            var response = await _httpClient.GetAsync($"crabadas/lending/history?borrower_address={address}&orderBy=transaction_time&order=desc&limit=2");

            var content = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode) return new MineResponse();


            content.TryDeserializeObject(out MineResponse result);

            return result;
        }

        public async Task<object> GetCanJoinTeamInfoAsync(string address)
        {
            var response = await _httpClient.GetAsync($"crabadas/can-join-team?user_address={address}");

            var content = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode) return new MineResponse();


            content.TryDeserializeObject(out MineResponse result);

            return result;
        }
    }
}

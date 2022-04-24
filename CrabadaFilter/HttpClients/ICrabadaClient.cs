using System.Threading.Tasks;
using CrabadaFilter.Models;

namespace CrabadaFilter.HttpClients
{
    public interface ICrabadaClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mineId">MineId</param>
        /// <returns></returns>
        Task<DataAnswer<MineResponse>> GetMineDetailsAsync(int mineId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns></returns>
        Task<object> GetLendingHistoryAsync(string address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">Wallet Address</param>
        /// <returns></returns>
        Task<object> GetCanJoinTeamInfoAsync(string address);
    }
}

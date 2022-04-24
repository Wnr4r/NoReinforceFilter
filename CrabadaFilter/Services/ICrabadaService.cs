using System.Threading.Tasks;
using CrabadaFilter.Models;

namespace CrabadaFilter.Services
{
    public interface ICrabadaService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mineId">MineId</param>
        /// <returns></returns>
        Task<MineResponse> GetMineDetailsAsync(int mineId);

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

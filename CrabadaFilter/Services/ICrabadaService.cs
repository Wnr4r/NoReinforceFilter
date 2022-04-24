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
        Task<MineDto> GetMineDetailsAsync(int mineId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">Wallet Address</param>
        /// <returns></returns>
        Task<CanJoinDto> GetCanJoinTeamInfoAsync(string address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns></returns>
        Task<LendingHistoryDto> GetLendingHistoryAsync(string address);

    }
}

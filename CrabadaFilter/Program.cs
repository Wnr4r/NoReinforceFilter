using CrabadaFilter.Extensions;
using CrabadaFilter.Models;
using CrabadaFilter.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CrabadaFilter
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        private static ICrabadaService _crabadaService;
        static async Task Main(string[] args)
        {

            var host = CreateHostBuilder(args).Build();

            // await host.RunAsync();
            _serviceProvider = host.Services;

            _crabadaService = _serviceProvider.GetService<ICrabadaService>();

            string response = string.Empty;
            do
            {
                Console.Write("Enter starting mine ID: ");
                int startMineID = int.Parse(Console.ReadLine());

                Console.Write("Enter how many additional mines to scan: ");
                int numberOfMines = int.Parse(Console.ReadLine());

                Console.Write("Enter last reinforcement time (Hours) threshold to query reinforcement history: ");
                double minReinforcementTransTimeHr = double.Parse(Console.ReadLine());

                int stopMineID = startMineID + numberOfMines;

                for (int i = startMineID; i <= stopMineID; i++)
                {

                    try
                    {
                        //check for owner address and see if it has not yet been looted
                        var mineDetails = await FilterOwnerAddress(i);
                        var address = mineDetails.Owner;
                        //if address is empty or miner has own crab for reinforcing, continue to next iteration
                        if (string.IsNullOrWhiteSpace(address) || await IsCrabAvailable(address)) continue;
                        //get miner's faction
                        var crabFaction = mineDetails.Defense_Team_Faction;
                        //get last time miner reinforced
                        var lastReinforceTimeDiffHHour = await FilterNoReinforceAddress(address);
                        //check to see if last reinforcement time is greater or equal to user specified time.
                        if (lastReinforceTimeDiffHHour >= minReinforcementTransTimeHr)
                        {
                            Console.WriteLine($"CrabFaction: {crabFaction} \t MineID: {i} \t OwnerAdress: {address} \t LastReinforceTime:  {lastReinforceTimeDiffHHour} Hrs");
                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine($"Error encountered: {e.Message}");
                    }

                    //sleep after each loop
                    Thread.Sleep(1000);
                }

                Console.WriteLine("\n Completed!!!!");
                Console.Write("\n Do you wish to check another mine ID series? Type yes to continue: ");
                response = Console.ReadLine();
            } while (response?.ToLower() == "yes");
        }

        /// <summary>
        /// Check wallet address of the miner.
        /// </summary>
        /// <param name="mineID">Mine ID.</param>
        /// <returns>wallet address of the miner in the given mine ID.</returns>
        public static async Task<MineDto> FilterOwnerAddress(int mineId) => await _crabadaService.GetMineDetailsAsync(mineId);

        /// <summary>
        /// Check the last time that miner reinforced.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <returns>last time miner reinforced. -1 - means, address sent is invalid or miner has never reinforced</returns>
        public static async Task<double> FilterNoReinforceAddress(string address)
        {
            //check to see that address returned is valid
            if (address.Length != 42)
            {
                return -1;
            }

            var currentDate = DateTime.UtcNow;
            var response = await _crabadaService.GetLendingHistoryAsync(address);
            //check to see if wallet has ever been to tarvern
            var totalRecord = response.TotalRecord;
            if (totalRecord <= 0)
            {
                return -1;
            }

            var lastReinforcementTime = response.Data[0].Transaction_Time;
            //var lastReinforcementTimeInHrf = DateTimeOffset.FromUnixTimeMilliseconds(lastReinforcementTime);
            DateTime lastReinforcementTimeInHrf = new DateTime(1970, 1, 1, 0, 0, 0, 0); //from start epoch time in HRF:Human Readable Forrmat
            lastReinforcementTimeInHrf = lastReinforcementTimeInHrf.AddSeconds(lastReinforcementTime); // update reinforcement using the latest tran_time
            double lastTransacTimeDiffHr = Math.Round((currentDate - lastReinforcementTimeInHrf).TotalDays) * 24;

            return lastTransacTimeDiffHr;
        }

        /// <summary>
        /// Check if miner has own crab to reinforce.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <returns>True if owner has crabs or address is invalid.</returns>
        public static async Task<bool> IsCrabAvailable(string address)
        {
            bool ownerCrabAvailableStatus = false;
            //check to see that address returned is valid
            if (string.IsNullOrWhiteSpace(address))
            {
                return true;
            }

            var response = await _crabadaService.GetCanJoinTeamInfoAsync(address);
            int totalRecord = response.TotalRecord;
            if (totalRecord > 0)
            {
                ownerCrabAvailableStatus = true;
            }
            return ownerCrabAvailableStatus;
        }

        //not used for now
        public static string QueryAPI(string url)
        {
            //Thread.Sleep(2000);
            //string urlu = $"https://idle-api.crabada.com/public/idle/crabadas/lending?borrower_address={address}&limit=100";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            var totalRecord = stuff.result.totalRecord;
            return string.Empty;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

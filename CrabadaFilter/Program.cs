using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace CrabadaFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            var config = builder.Build();


            string response = string.Empty;
            do
            {
                Console.Write("Enter starting mine ID: ");
                int startMineID = Int32.Parse(Console.ReadLine());

                Console.Write("Enter how many additional mines to scan: ");
                int numberOfMines = Int32.Parse(Console.ReadLine());

                Console.Write("Enter last reinforcement time (Hours) threshold to query reinforcement history: ");
                double minReinforcemnentTransTimeHr = Double.Parse(Console.ReadLine());

                int stopMineID = startMineID + numberOfMines;

                for (int i = startMineID; i <= stopMineID; i++)
                {

                    try
                    {
                        string address = filterOwnerAddress(i);
                        string crabFaction = minerFaction(i);
                        double lastReinforceTimeDiffHHour = filterNoReinforceAddress(address);

                        //if address is empty or miner has own crab for reinforcing, continue to next iteration
                        if (string.IsNullOrWhiteSpace(address) || isCrabAvailable(address)) continue;

                        //check to see if last reinforcement time is greater or equal to user required time.
                        if (lastReinforceTimeDiffHHour >= minReinforcemnentTransTimeHr)
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
            } while (response.ToLower() == "yes");
        }

        /// <summary>
        /// Check wallet address of the miner.
        /// </summary>
        /// <param name="mineID">Mine ID.</param>
        /// <returns>wallet address of the miner in the given mine ID.</returns>
        public static string filterOwnerAddress(int mineID)
        {
            //Thread.Sleep(2000);
            string url = $"https://idle-api.crabada.com/public/idle/mine/{mineID}";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            string ownerAddress = stuff.result.owner;
            var attackTeamID = stuff.result.attack_team_id;
            //Console.WriteLine(ownerAddress);
            //check to see that team is not looted
            //string.IsNullOrWhiteSpace(attackTeamID);
            return (attackTeamID > 0) ? string.Empty : ownerAddress;
            //if (attackTeamID > 0)
            //{
            //    return "";
            //}
            //else
            //{
            //    return ownerAddress;
            //}

        }

        /// <summary>
        /// Check if miner has own crab to reinforce.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <returns>reinforcement history of the input address. 0 - means, no reinforement history</returns>
        public static double filterNoReinforceAddress(string address)
        {
            //check to see that address returned is valid
            if (address.Length != 42)
            {
                return -1;
            }
            //sleep to avoid ban
            //Thread.Sleep(1000);
            DateTime currentDate = DateTime.Now;
            string url = $"https://idle-api.crabada.com/public/idle/crabadas/lending/history?borrower_address={address}&orderBy=transaction_time&order=desc&limit=2";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            //check to see if wallet has ever been to tarvern
            var totalRecord = stuff.result.totalRecord;
            if (totalRecord <= 0)
            {
                return -1;
            }
            double lastReinforcementTime = stuff.result.data[0].transaction_time;
            DateTime lastReinforcementTimeInHRF = new DateTime(1970, 1, 1, 0, 0, 0, 0); //from start epoch time in HRF:Human Readable Forrmat
            lastReinforcementTimeInHRF = lastReinforcementTimeInHRF.AddSeconds(lastReinforcementTime); // update reinforcement using the latest tran_time
            double lastTransacTimeDiffHr = Math.Round(((currentDate - lastReinforcementTimeInHRF).TotalDays) * 24);

            // calibrate epoch to HRF conversion
            return lastTransacTimeDiffHr - 3;
        }

        /// <summary>
        /// Check if miner has own crab to reinforce.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <returns>True if owner has crabs or address is invalid.</returns>
        public static bool isCrabAvailable(string address)
        {
            bool ownerCrabAvailableStatus = false;
            //check to see that address returned is valid
            if (string.IsNullOrWhiteSpace(address))
            {
                ownerCrabAvailableStatus = true;
                return ownerCrabAvailableStatus;
            }
            //sleep to avoid ban
            //Thread.Sleep(1000);
            string url = $"https://idle-api.crabada.com/public/idle/crabadas/can-join-team?user_address={address}";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            int totalRecord = stuff.result.totalRecord;
            if (totalRecord > 0)
            {
                ownerCrabAvailableStatus = true;
            }
            return ownerCrabAvailableStatus;
        }


        /// <summary>
        /// Checks miner's crab faction
        /// </summary>
        /// <param name="mineID">Miner's ID</param>
        /// <returns>Miner's crab faction</returns>
        public static string minerFaction(int mineID)
        {

            //Thread.Sleep(1000);
            string url = $"https://idle-api.crabada.com/public/idle/mine/{mineID}";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            string teamFaction = stuff.result.defense_team_faction;

            return teamFaction;

        }

        //not used for now
        public static string queryAPI(string url)
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
    }
}

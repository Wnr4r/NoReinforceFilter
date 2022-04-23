using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CrabadaFilter
{
    class Program
    {
        static void Main(string[] args)
        {

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
                        //Console.WriteLine($"Currently Scanning Mine: {i}");
                        //check for owner address and see if it has not yet been looted
                        string address = filterOwnerAddress(i);
                        //if address is empty or miner has own crab for reinforcing, continue to next iteration
                        if (string.IsNullOrWhiteSpace(address) || isCrabAvailable(address)) continue;
                        //get miner's faction
                        string crabFaction = minerFaction(i);
                        //get last time miner reinforced
                        double lastReinforceTimeDiffHHour = filterNoReinforceAddress(address);
                        //check to see if last reinforcement time is greater or equal to user specified time.
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
        /// Check the last time that miner reinforced.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <returns>last time miner reinforced. -1 - means, address sent is invalid or miner has never reinforced</returns>// in game crabs
        public static bool isCrabsInGame(string address)
        {
            bool ownerCrabInGameStatus = false;

            if (string.IsNullOrWhiteSpace(address))
            {
                ownerCrabInGameStatus  = true;
                return ownerCrabInGameStatus;
            }

            string url = $"https://idle-api.crabada.com/public/idle/crabadas/in-game?user_address={address}&page=1&limit=15&order=desc&orderBy=battle_point";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            int totalRecord = stuff.result.totalRecord;

            for (int i=0; i < totalRecord; i++)
            {
                string teamID = stuff.result.data[i].team_id;
                //Console.WriteLine($" teamID: {teamID}");
                if(String.IsNullOrEmpty(teamID))
                {
                    ownerCrabInGameStatus = true;
                }
            }

            return ownerCrabInGameStatus;
            
        }
        public static double filterNoReinforceAddress(string address)
        {
            //check to see that address returned is valid
            if (address.Length != 42)
            {
                return -1;
            }
            //sleep to avoid ban
            //Thread.Sleep(1000);
            DateTime currentDate = DateTime.UtcNow;
            string url = $"https://idle-api.crabada.com/public/idle/crabadas/lending/history?borrower_address={address}&orderBy=transaction_time&order=desc&limit=2";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            //check to see if wallet has ever been to tarvern
            var totalRecord = stuff.result.totalRecord;
            if(totalRecord <= 0)
            {
                return -1;
            }
            double lastReinforcementTime = stuff.result.data[0].transaction_time;
            DateTime lastReinforcementTimeInHRF  = new DateTime(1970, 1, 1, 0, 0, 0, 0); //from start epoch time in HRF:Human Readable Forrmat
            lastReinforcementTimeInHRF = lastReinforcementTimeInHRF.AddSeconds(lastReinforcementTime); // update reinforcement using the latest tran_time
            double lastTransacTimeDiffHr =  Math.Round(((currentDate - lastReinforcementTimeInHRF).TotalDays) * 24);
            
            return lastTransacTimeDiffHr;
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


            if (totalRecord > 0 || areCrabsInGame(address) == true)
            {
                ownerCrabAvailableStatus = true;
            }


            return ownerCrabAvailableStatus;

            
        }

        // checks in game crabs
        public static bool areCrabsInGame(string address)
        {
            bool ownerCrabInGameStatus = false;

            if (string.IsNullOrWhiteSpace(address))
            {
                ownerCrabInGameStatus  = true;
                return ownerCrabInGameStatus;
            }

            string url = $"https://idle-api.crabada.com/public/idle/crabadas/in-game?user_address={address}&page=1&limit=15&order=desc&orderBy=battle_point";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            int totalRecord = stuff.result.totalRecord;

            for (int i=0; i < totalRecord; i++)
            {
                string teamID = stuff.result.data[i].team_id;
                //Console.WriteLine($" teamID: {teamID}");
                if(String.IsNullOrEmpty(teamID))
                {
                    ownerCrabInGameStatus = true;
                }
            }

            return ownerCrabInGameStatus;
            
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
        public static string queryAPI(string url )
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

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CrabadaFilter
{
    //global variables class
    public static class Globals
    {
        //swimmer API baseUrl - https://idle-game-api.crabada.com/public/idle
        //AVAX APi baseurl - https://idle-api.crabada.com/public/idle
        public const string baseURL = "https://idle-game-api.crabada.com/public/idle";
        public const string baseURL2 = "https://idle-api.crabada.com/public/idle";
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string response = string.Empty;
                do
                {

                    Console.Write("Enter starting mine ID: ");
                    int startMineID = Int32.Parse(Console.ReadLine().Trim());

                    Console.Write("Enter how many additional mines to scan: ");
                    int numberOfMines = Int32.Parse(Console.ReadLine().Trim());

                    Console.Write("Enter last reinforcement time (Hours) threshold to query reinforcement history: ");
                    double minReinforcemnentTransTimeHr = Double.Parse(Console.ReadLine().Trim());

                    Console.Write("Enter 1st faction that you want to loot (leave empty if you dont have preferred faction): ");
                    string lootFaction1 = Console.ReadLine().ToUpper().Trim();

                    Console.Write("Enter 2nd faction that you want to loot (leave empty if you dont have preferred faction): ");
                    string lootFaction2 = Console.ReadLine().ToUpper().Trim();

                    int stopMineID = startMineID + numberOfMines;

                    for (int i = startMineID; i <= stopMineID; i++)
                    {

                        try
                        {
                            //Console.WriteLine($"Currently Scanning Mine: {i}");
                            //get miner's faction
                            string crabFaction = minerFaction(i);
                            if ((lootFaction1 == crabFaction || lootFaction2 == crabFaction) || (lootFaction1 == "" && lootFaction2 == ""))
                            { 
                                //check for owner address and see if it has not yet been looted
                                string address = filterOwnerAddress(i);
                                //get last time miner reinforced
                                double lastReinforceTimeDiffHHour = filterNoReinforceAddress(address);
                                //check to see if last reinforcement time is greater or equal to user specified time.
                                if (lastReinforceTimeDiffHHour < minReinforcemnentTransTimeHr) continue;
                                //if address is empty or miner has own crab for reinforcing, continue to next iteration
                                if (string.IsNullOrWhiteSpace(address) || isCrabAvailable(address)) continue;
                                //output if all conditions are met
                                Console.WriteLine($"Faction: {crabFaction} \t MineID: {i} \t Address: {address} \t LastReinforced:  {lastReinforceTimeDiffHHour} Hrs");
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
                } while (response.ToLower().Trim()
                            .Equals("yes"));
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured: {e.Message}");
            }
            finally
            {
                Console.Write("Press enter to close console");
                Console.ReadLine();
            }
            
        }
        #region Functions

        /// <summary>
        /// Check wallet address of the miner.
        /// </summary>
        /// <param name="mineID">Mine ID.</param>
        /// <returns>wallet address of the miner in the given mine ID.</returns>
        public static string filterOwnerAddress(int mineID)
        {
            //Thread.Sleep(2000);
            string url = Globals.baseURL+ $"/mine/{mineID}";
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
        /// <returns>last time miner reinforced. -1 - means, address sent is invalid or miner has never reinforced</returns>
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
            string url = Globals.baseURL + $"/crabadas/lending/history?borrower_address={address}&orderBy=transaction_time&order=desc&limit=2";
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
            string url = Globals.baseURL + $"/crabadas/can-join-team?user_address={address}";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            int totalRecord = stuff.result.totalRecord;
            

            // check to see if miner has reserve crab in wallet or a reserve crab already in game
            if (totalRecord > 0 || areCrabsInGame(address))
            {
                ownerCrabAvailableStatus = true;
            }
            return ownerCrabAvailableStatus;
        }


        // recently added function - 23-04-2022 : checks for crabs in game
        public static bool areCrabsInGame(string address)
        {
            bool ownerCrabInGameStatus = false;
            int totalRecord;
            int remainder;

            if (string.IsNullOrWhiteSpace(address))
            {
                ownerCrabInGameStatus  = true;
                return ownerCrabInGameStatus;
            }

            string url = Globals.baseURL + $"/crabadas/in-game?user_address={address}&page=1&limit=100&order=desc&orderBy=mine_point";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            // get the totalPages
            int totalPages = stuff.result.totalPages;
            // current page
            int page = stuff.result.page;
            totalRecord = stuff.result.totalRecord;
            remainder = totalRecord % 3;

            //Console.WriteLine($"Remainder {remainder}");
            if (remainder != 0)
            {
                ownerCrabInGameStatus = true;
                return ownerCrabInGameStatus;
            }

            // for a single in-game page
            if (totalPages == page)
            {
                // we need to limit the totalRecord to the index that can be processed
                for (int i = 0; i < totalRecord; i++)
                {
                    string teamID = stuff.result.data[i].team_id;
                    //Console.WriteLine($" teamID: {teamID}");
                    if (String.IsNullOrEmpty(teamID))
                    {
                        ownerCrabInGameStatus = true;
                        return ownerCrabInGameStatus;
                    }
                }
            }

            // handles in-game multiple pages when totalPages is > page
            // this will be rarely executed bcos the probability of lots crabbers having more than 100 records is low
            if (totalPages > 1)
            {
                // loop through each page by calling the api - start from page 1
                for (int i = 1; i <= totalPages; i++)
                {
                    // call the api starting from page 1
                    url = Globals.baseURL + $"/crabadas/in-game?user_address={address}&page={i}&limit=100&order=desc&orderBy=mine_point";
                    client = new WebClient();
                    client.Headers.Add("User-Agent: Other");
                    content = client.DownloadString(url);
                    stuff = JObject.Parse(content);
                    JArray dataArray = (JArray)stuff.result.data;
                    //Console.WriteLine($"dataArray: {dataArray.GetType()}");
                    //Console.WriteLine($"dataArray Length: {dataArray.Count}");

                    //loop through the data for each page
                    for (int k = 0; k < dataArray.Count; k++)
                    {
                        string teamID = stuff.result.data[k].team_id;

                        if (String.IsNullOrEmpty(teamID))
                        {
                            ownerCrabInGameStatus = true;
                            return ownerCrabInGameStatus;
                        }
                    }

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
            string url = Globals.baseURL + $"/mine/{mineID}";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            string teamFaction = stuff.result.defense_team_faction;
            return teamFaction;
            
        }
        #endregion
    }
}

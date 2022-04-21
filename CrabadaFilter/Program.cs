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
            Console.Write("Enter starting mine ID: ");
            int startMineID = Int32.Parse(Console.ReadLine());
            Console.Write("Enter how many additional mines to scan: ");
            int numberOfMines = Int32.Parse(Console.ReadLine());
            //int numberOfMines = 5;
            //int startMineID = 4840793;
            int stopMineID = startMineID + numberOfMines;

            for (int i= startMineID; i<= stopMineID; i++ )
            {
                string address = filterOwnerAddress(i);
                //if (address == "") break;
                int totalRecord = filterNoReinforceAddress(address);
                if (totalRecord == 0)
                {
                    Console.WriteLine($"MineID: {i} \t OwnerAdress: {address}");
                }

            }
            Console.WriteLine("\n Completed, press any key to exit");
            //Console.ReadLine();
        }

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
            if (attackTeamID > 0)
            {
                return "";
            }
            else
            {
                return ownerAddress;
            }
            
        }

        public static int filterNoReinforceAddress(string address)
        {
            //check to see that address returned is valid
            if (address.Length != 42)
            {
                return -1;
            }
            //sleep to avoid ban
            Thread.Sleep(2000);
            string url = $"https://idle-api.crabada.com/public/idle/crabadas/lending?borrower_address={address}&limit=100";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            int totalRecord = stuff.result.totalRecord;
            return totalRecord;
        }

        //not used for now
        public static string queryAPI(string url )
        {
            Thread.Sleep(2000);
            //string urlu = $"https://idle-api.crabada.com/public/idle/crabadas/lending?borrower_address={address}&limit=100";
            var client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            var content = client.DownloadString(url);
            dynamic stuff = JObject.Parse(content);
            var totalRecord = stuff.result.totalRecord;
            return "";
        }
     }
 }

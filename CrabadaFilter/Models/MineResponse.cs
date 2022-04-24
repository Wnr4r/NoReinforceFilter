namespace CrabadaFilter.Models
{
    public class MineResponse
    {
        public string Owner { get; set; }

        public int Team_Id { get; set; }

        public string Status { get; set; }

        public string Defense_Team_Faction { get; set; }

        public int? Attack_Team_Id{ get; set; }
    }
}

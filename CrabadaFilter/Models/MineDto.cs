namespace CrabadaFilter.Models
{
    /// <summary>
    /// Mine endpoint response model
    /// https://idle-api.crabada.com/public/idle/mine/{mineID}
    /// </summary>
    public class MineDto
    {
        public string Owner { get; set; } = string.Empty;

        public int Team_Id { get; set; }

        public string Status { get; set; }

        public string Defense_Team_Faction { get; set; }

        public int? Attack_Team_Id { get; set; }
    }
}

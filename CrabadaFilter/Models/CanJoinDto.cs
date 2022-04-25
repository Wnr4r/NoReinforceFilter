using System.Collections.Generic;

namespace CrabadaFilter.Models
{
    /// <summary>
    /// Can join team response model
    /// https://idle-api.crabada.com/public/idle/crabadas/can-join-team?user_address={address}
    /// </summary>
    public class CanJoinDto : PaginationDto
    {
        public IList<CanJoinResponse> Data { get; set; }

    }

    public class CanJoinResponse
    {
        public int Crabada_Id { get; set; }
        public int Id { get; set; }
        public string Crabada_Name { get; set; }
        public string Owner { get; set; }

        public int Crabada_Type { get; set; }
        public int Crabada_Class { get; set; }

        public int Class_Id { get; set; }

        public string Class_Name { get; set; }

        public int Is_Origin { get; set; }

        public int Is_Genesis { get; set; }

        public int Legend_Number { get; set; }

        public int Pure_Number { get; set; }

        public string Photo { get; set; }

        public int Hp { get; set; }

        public int Speed { get; set; }
        public int Damage { get; set; }
        public int Critical { get; set; }
        public int Armor { get; set; }
        public int Battle_Point { get; set; }
        public int Time_Point { get; set; }
        public int Mine_Point { get; set; }
    }
}

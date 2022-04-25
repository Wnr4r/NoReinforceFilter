using System.Collections.Generic;

namespace CrabadaFilter.Models {
    /// <summary>
    /// Lending History Response model
    /// https://idle-api.crabada.com/public/idle/crabadas/lending/history?borrower_address={address}&orderBy=transaction_time&order=desc&limit=
    /// </summary>
    public class LendingHistoryDto : PaginationDto
    {

        public IList<LendingHistoryResponse> Data { get; set; }
    }

    public class LendingHistoryResponse
    {
        public int Crabada_Id { get; set; }
        public string Transaction { get; set; }
        public double Price { get; set; }
        public string Lender { get; set; }
        public string Borrower { get; set; }
        public int Game_Id { get; set; }
        public string Borrower_Name { get; set; }

        public long Transaction_Time { get; set; }
    }
}

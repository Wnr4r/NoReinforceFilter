using System.Collections.Generic;

namespace CrabadaFilter.Models {
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

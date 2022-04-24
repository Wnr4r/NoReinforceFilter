namespace CrabadaFilter.Models {
    public class PaginationDto {
        public int TotalRecord { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}

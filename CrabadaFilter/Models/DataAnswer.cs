namespace CrabadaFilter.Models
{
    public class DataAnswer<T>
    {
        public string Error_Code { get; set; }

        public string Message { get; set; }

        public T Result { get; set; }

    }
}

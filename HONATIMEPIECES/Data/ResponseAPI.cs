namespace HONATIMEPIECES.Data
{
    public class ResponseAPI<T>
    {
        public string Message { get; set; }
        public T Result { get; set; }
        public string Error { get; set; }
        public int StatusCode { get; set; }
    }
}

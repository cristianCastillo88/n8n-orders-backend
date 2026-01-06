namespace WebApiPizzeria.DTOs
{
    public class BaseResponseDto<T> where T : class
    {
        public bool Success { get; set; }
        public T Response { get; set; }

        public BaseResponseDto(bool success, T response)
        {
            Success = success;
            Response = response;
        }
    }
}

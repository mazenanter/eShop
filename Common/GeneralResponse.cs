using System.Text.Json.Serialization;

namespace eShop.Common
{
    public class GeneralResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }

        public static GeneralResponse<T> Success(T data, string message = "Success")
        => new GeneralResponse<T> { IsSuccess = true, Data = data, Message = message };

        public static GeneralResponse<T> Failure(string message, List<string>? errors = null)
            => new GeneralResponse<T> { IsSuccess = false, Message = message, Errors = errors ?? new List<string>() };
    }
}

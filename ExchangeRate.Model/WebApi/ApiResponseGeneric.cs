namespace Model.ApiResponse;

public class ApiResponse<T> : ApiResponse where T : class
{
    public ApiResponse(T data)
    {
        this.Success = true;
        this.Data = data;
        this.Message = string.Empty;
    }

    public ApiResponse(T data, string message)
    {
        this.Success = true;
        this.Data = data;
        this.Message = message;
    }

    public ApiResponse(T data, string message, int eventId)
    {
        this.Success = true;
        this.Data = data;
        this.Message = message;
        this.EventId = eventId;
    }

    public ApiResponse(T? data, bool success, string message)
    {
        this.Success = success;
        this.Data = data;
        this.Message = message;
    }
    

    public T? Data { get; set; }
}
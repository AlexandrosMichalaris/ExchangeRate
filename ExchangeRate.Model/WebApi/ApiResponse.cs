namespace Model.ApiResponse;

public class ApiResponse
{
    public bool Success { get; set; }

    /// <summary>
    /// Text Message response
    /// </summary>
    public string Message { get; set; }

    public int? EventId { get; set; }

    public ApiResponse()
    {

    }

    public ApiResponse(string message)
    {
        this.Success = true;
        this.Message = message;
    }

    public ApiResponse(string message, int eventId)
    {
        this.Success = true;
        this.EventId = eventId;
    }

    public ApiResponse(string message, bool success)
    {
        this.Success = success;
        this.Message = message;
    }
    
    public ApiResponse(string message, bool success, int eventId)
    {
        this.Success = success;
        this.Message = message;
        this.EventId = eventId;
    }
}
using System.Net;
using System.Text.Json;
using Model.ApiResponse;

namespace ExchangeRate.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    #region Ctor
    
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    #endregion

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new ApiResponse(ex.Message, false, response.StatusCode);
            
            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }
}
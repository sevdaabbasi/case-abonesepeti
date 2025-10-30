using Serilog;
namespace Auth.Api.Middlewares;

public class ErrorHandlerMiddleware
{
    public readonly RequestDelegate next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        
        
        catch (Exception ex)
        {             
            Log.Error(ex, $"Path: {context.Request.Path} " +
                          $"Method: {context.Request.Method}" +
                          $" QueryString: {context.Request.QueryString} " +
                          $"StatusCode: {context.Response.StatusCode}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(ex.Message);
        }
    }
}

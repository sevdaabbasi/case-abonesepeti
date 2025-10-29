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
            Log.Fatal($"Path: {context.Request.Path} {Environment.NewLine}" +
                      $"Method: {context.Request.Method} {Environment.NewLine}" +
                      $"QueryString: {context.Request.QueryString} {Environment.NewLine}" +
                      $"StatusCode: {context.Response.StatusCode} {Environment.NewLine}" +
                      $"Exception: {ex.Message}");

            Log.Fatal(ex, "İsteği işlerken işlenmemiş bir istisna oluştu.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(ex.Message);
        }
    }
}

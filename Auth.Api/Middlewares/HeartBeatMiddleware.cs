using System.Net;
namespace Auth.Api.Middlewares;

public class HeartBeatMiddleware
{
    public readonly RequestDelegate next;

    public HeartBeatMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync("Healthy");
            return;
        }

        
        await next(context);
    }
}

using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json.Linq;
using TaskManagementSystem.Bll.Services.Interfaces;

namespace TaskManagementSystem.Interceptors;

public class RateLimiterInterceptor : Interceptor
{
    private readonly IRateLimiterService _rateLimiterService;
    public RateLimiterInterceptor(IRateLimiterService rateLimiterService)
    {
        _rateLimiterService = rateLimiterService;
    }
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        if (request.ToString() is not null)
        {
            var userIp = JObject.Parse(request.ToString())["userId"].ToString();
            Console.WriteLine(userIp);
            await _rateLimiterService.ThrowIfTooManyRequests(userIp, context.CancellationToken);
        }

        return await continuation(request, context);
    }

}
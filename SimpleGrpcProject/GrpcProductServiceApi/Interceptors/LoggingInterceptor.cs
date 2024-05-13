using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcProductServiceApi.Interceptors;

public class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation($"Request to: {context.Method}");
        var response = await continuation(request, context);
        logger.LogInformation($"Response from: {context.Method}; Status: {context.Status}");
        return response;
    }
}
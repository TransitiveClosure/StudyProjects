using Domain.Exceptions;
using Grpc.Core;

namespace GrpcProductServiceApi.Interceptors;

public static class ExceptionHelper
{
    public static RpcException Handle<T>(this Exception exception, ILogger<T> logger) =>
        exception switch
        {
            ProductNotFoundException dontExistException => HandleProductDontExistException(dontExistException,
                logger),
            ProductAlreadyExistException alreadyExistException => HandleProductAlreadyExistException(alreadyExistException,
                logger),
            RpcException rpcException => HandleRpcException(rpcException, logger),
            _ => HandleDefault(exception, logger)
        };

    private static RpcException HandleProductDontExistException<T>(
        ProductNotFoundException exception, ILogger<T> logger)
    {
        logger.LogError(exception.Message);
        var status = new Status(StatusCode.NotFound, exception.Message);
        return new RpcException(status);
    }
    
    private static RpcException HandleProductAlreadyExistException<T>(
        ProductAlreadyExistException exception, ILogger<T> logger)
    {
        logger.LogError(exception.Message);
        var status = new Status(StatusCode.AlreadyExists, exception.Message);
        return new RpcException(status);
    }
    
    private static RpcException HandleRpcException<T>(RpcException exception, ILogger<T> logger)
    {
        logger.LogError(exception.Message);
        return exception;
    }
    
    private static RpcException HandleDefault<T>(Exception exception, ILogger<T> logger)
    {
        logger.LogError(exception.Message);
        var status = new Status(StatusCode.Internal, exception.Message);
        return new RpcException(status);
    }
}
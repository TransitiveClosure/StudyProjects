using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcProductServiceApi.Interceptors;

public class ValidationInterceptor(IServiceProvider serviceProvider) : Interceptor
{
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();
        var validatorResult = validator?.Validate(request);
        if (validatorResult is not null && !validatorResult.IsValid)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    $"{string.Join(";", validatorResult.Errors.Select(err => err.ErrorMessage))}"
                ));
        }
        return continuation(request, context);
    }
}
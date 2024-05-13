using FluentValidation;

namespace GrpcProductServiceApi.Validators;

public class GetProductByIdRequestValidator: AbstractValidator<GetProductByIdRequest>
{
    public GetProductByIdRequestValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThanOrEqualTo(ValidatorConfig.MinProductId).LessThanOrEqualTo(ValidatorConfig.MaxProductId);
    }
}
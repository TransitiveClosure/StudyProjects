using FluentValidation;

namespace GrpcProductServiceApi.Validators;

public class UpdateProductPriceRequestValidator : AbstractValidator<UpdateProductPriceRequest>
{
    public UpdateProductPriceRequestValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThanOrEqualTo(ValidatorConfig.MinProductId).LessThanOrEqualTo(ValidatorConfig.MaxProductId);
        RuleFor(request => request.NewPrice)
            .GreaterThanOrEqualTo(0);
    }
}
using FluentValidation;

namespace GrpcProductServiceApi.Validators;

public class AddNewProductRequestValidator : AbstractValidator<AddNewProductRequest>
{
    public AddNewProductRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().Must(str => !string.IsNullOrWhiteSpace(str));
        RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Weight).GreaterThanOrEqualTo(0);
        RuleFor(request => request.DateOfCreation.Nanos)
            .LessThanOrEqualTo(ValidatorConfig.MaxTimestampNanos).GreaterThanOrEqualTo(0);
        RuleFor(request => request.DateOfCreation.Seconds).LessThan(ValidatorConfig.MaxTimestampSeconds);
        RuleFor(request => request.WarehouseId)
            .GreaterThanOrEqualTo(ValidatorConfig.MinWarehouseId).LessThanOrEqualTo(ValidatorConfig.MaxWarehouseId);
        RuleFor(request => request.Category).NotEqual(ProductCategoryGrpc.Unspecified);
    }
}
using FluentValidation;

namespace GrpcProductServiceApi.Validators;

public class GetProductsByFiltersRequestValidator : AbstractValidator<GetProductsByFiltersRequest>
{
    public GetProductsByFiltersRequestValidator()
    {
        RuleFor(request => request.StartDate.Nanos)
            .LessThanOrEqualTo(ValidatorConfig.MaxTimestampNanos).GreaterThanOrEqualTo(0);
        RuleFor(request => request.StartDate.Seconds).LessThan(ValidatorConfig.MaxTimestampSeconds);
        RuleFor(request => request.EndDate.Nanos)
            .LessThanOrEqualTo(ValidatorConfig.MaxTimestampNanos).GreaterThanOrEqualTo(0);
        RuleFor(request => request.EndDate.Seconds).LessThan(ValidatorConfig.MaxTimestampSeconds);
        RuleFor(request => request.WarehouseId)
            .GreaterThanOrEqualTo(ValidatorConfig.MinWarehouseId).LessThanOrEqualTo(ValidatorConfig.MaxWarehouseId);
        RuleFor(request => request.PageNumber)
            .GreaterThanOrEqualTo(1);
        RuleFor(request => request.PageSize)
            .GreaterThanOrEqualTo(0);
        RuleFor(request => request.Category).NotEqual(ProductCategoryGrpc.Unspecified);
    }
}
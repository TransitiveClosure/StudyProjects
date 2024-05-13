namespace GrpcProductServiceApi.Validators;

public static class ValidatorConfig
{
    public static ulong MinProductId { get; } = 100000000000;
    public static ulong MaxProductId { get; } = 999999999999;
    public static ulong MinWarehouseId { get; } = 100000000000;
    public static ulong MaxWarehouseId { get; } = 999999999999;
    
    public static int MaxTimestampNanos { get; } = 999999999;
    public static long MaxTimestampSeconds { get; } = long.MaxValue;
    
}
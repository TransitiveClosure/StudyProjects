namespace Domain.Exceptions;

public class ProductNotFoundException(ulong productId)
    : Exception(string.Format($"Exception: not found product with id: {productId}"));
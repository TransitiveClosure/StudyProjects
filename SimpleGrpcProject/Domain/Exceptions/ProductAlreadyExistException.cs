namespace Domain.Exceptions;

public class ProductAlreadyExistException(ulong productId) : 
    Exception(string.Format($"Exception: already exist product with id: {productId}"));
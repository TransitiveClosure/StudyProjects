using Domain.Interfaces;

namespace DataAccess;

public class ProductIdGenerator : IProductIdGenerator
{
    private ulong _id = 100000000000;
    private readonly object _locker = new object();
    public ulong GetId()
    { 
        lock (_locker)
        {
            return _id++;
        }
    }
}

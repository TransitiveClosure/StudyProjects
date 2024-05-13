namespace TaskManagementSystem.Bll.Exceptions;

public class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string clientIp) 
        : base(string.Format($"Exception: Too many requests from clientIP: {clientIp}")){}
}
    
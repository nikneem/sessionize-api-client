namespace Sessionize.Api.Client.Exceptions;
public class SessionizeApiClientException : Exception
{
    public ErrorCode ErrorCode { get; }

    public SessionizeApiClientException(ErrorCode errorCode, string message, Exception? innerException = null) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
    public SessionizeApiClientException(ErrorCode errorCode, Exception? innerException = null) : base(errorCode.Message, innerException)
    {
        ErrorCode = errorCode;
    }
}
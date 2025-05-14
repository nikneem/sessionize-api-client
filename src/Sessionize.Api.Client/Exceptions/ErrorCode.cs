namespace Sessionize.Api.Client.Exceptions;

public sealed class ErrorCode
{


    public static readonly ErrorCode InvalidConfiguration = new(nameof(InvalidConfiguration), "The configuration is invalid, please check the configuration values for Sessionize.BaseUrl and Sessionize.ApiId");
    public static readonly ErrorCode SessionizeResponseEmpty = new(nameof(SessionizeResponseEmpty), "The response from Sessionize was empty");
    public static readonly ErrorCode DeserializationFailed = new(nameof(DeserializationFailed), "Failed to deserialize the response from Sessionize");

    public string Code { get; }
    public string Message { get; }

    private ErrorCode(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
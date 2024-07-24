using System.Net;

namespace c6_boss_web_framework.framework.requests;

public interface IResponseEntity
{
    HttpStatusCode StatusCode { get; set; }
    string ContentType { get; set; }
    string? ContentEncoding { get; set; }
    object? GetBody();
    Dictionary<string, string> Headers { get; set; }
}

public class ResponseEntity : IResponseEntity
{
    private Dictionary<string, string> _headers = new();

    public Dictionary<string, string> Headers
    {
        get => _headers;
        set => _headers = value ?? throw new ArgumentNullException(nameof(value));
    }

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NoContent;
    public string ContentType { get; set; } = "application/json";
    public string? ContentEncoding { get; set; }

    public object? Body { get; set; }
    public object? GetBody() => Body;


    public void AddHeader(string key, string value)
    {
        _headers[key] = value;
    }
}
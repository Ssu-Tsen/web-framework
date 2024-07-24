using System.Net;
using System.Text.Json;
using c6_boss_web_framework.framework.http_body_serializers;

namespace c6_boss_web_framework.framework.requests;

public static class HttpRequest
{
    private static HttpBodySerializerChain? _serializer;

    public static void SetHttpBodySerializer(HttpBodySerializerChain? serializer)
    {
        _serializer = serializer;
    }

    public static async Task<T> ReadBodyAsObject<T>(this HttpListenerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.ContentType);
        if (!request.HasEntityBody)
        {
            throw new InvalidOperationException("Request doesn't have a body.");
        }

        return await _serializer!.Deserialize<T>(request.ContentType, request.InputStream);
    }
}
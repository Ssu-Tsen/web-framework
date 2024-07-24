using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using c6_boss_web_framework.framework.exceptions;

namespace c6_boss_web_framework.framework.http_body_serializers;

public class HttpBodySerializerChain(List<HttpBodySerializer> serializers)
{
    public void AddSerializer(HttpBodySerializer serializer)
    {
        serializers.Add(serializer);
    }

    public string Serialize<T>(T obj, string contentType)
    {
        foreach (var serializer in serializers.Where(serializer => serializer.IsResponsible(contentType)))
        {
            return serializer.SerializeObj(obj, contentType);
        }

        throw new UnsupportedSerializationTypeException(
            $"The content type '{contentType}' is not supported.");
    }

    public async Task<T> Deserialize<T>(string contentType, Stream inStream)
    {
        foreach (var deserializer in serializers.Where(deserialize => deserialize.IsResponsible(contentType)))
        {
            using StreamReader reader = new StreamReader(inStream);
            var body = await reader.ReadToEndAsync();
            return deserializer.DeserializeObj<T>(body);
        }

        throw new AggregateException(
            $"The content type '{contentType}' is not supported.");
    }
}

public abstract class HttpBodySerializer
{
    public abstract bool IsResponsible(string contentType);

    public abstract T DeserializeObj<T>(string body);
    public abstract string SerializeObj<T>(T obj, string contentType);
}

internal class HttpBodyJsonSerializer : HttpBodySerializer
{
    public override bool IsResponsible(string contentType)
    {
        return contentType is "application/json";
    }

    public override T DeserializeObj<T>(string body)
    {
        return JsonSerializer.Deserialize<T>(body) ??
               throw new InvalidOperationException(
                   $"Cannot deserialize with type = \"application/json\", body = {body}");
    }

    public override string SerializeObj<T>(T obj, string contentType)
    {
        return JsonSerializer.Serialize(obj);
    }
}

internal class HttpBodyXmlSerializer : HttpBodySerializer
{
    public override bool IsResponsible(string contentType)
    {
        return contentType is "application/xml";
    }

    public override T DeserializeObj<T>(string body)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringReader = new StringReader(body);
        return (T?)serializer.Deserialize(stringReader) ?? throw new InvalidOperationException();
    }

    public override string SerializeObj<T>(T obj, string contentType)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }
}
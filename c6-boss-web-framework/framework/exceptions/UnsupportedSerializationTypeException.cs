namespace c6_boss_web_framework.framework.exceptions;

public class UnsupportedSerializationTypeException : Exception
{
    public UnsupportedSerializationTypeException()
    {
    }

    public UnsupportedSerializationTypeException(string message) : base(message)
    {
    }

    public UnsupportedSerializationTypeException(string message, Exception inner) : base(message, inner)
    {
    }
}
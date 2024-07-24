namespace c6_boss_web_framework.framework.exceptions;

public class OperationForbiddenException : Exception
{
    public OperationForbiddenException()
    {
    }

    public OperationForbiddenException(string message) : base(message)
    {
    }

    public OperationForbiddenException(string message, Exception inner) : base(message, inner)
    {
    }
}
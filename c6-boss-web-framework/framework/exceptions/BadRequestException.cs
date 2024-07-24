namespace c6_boss_web_framework.framework.exceptions;

public class BadRequestException : Exception
{
    public BadRequestException()
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class ResourceNotFoundException : BadRequestException
{
    public ResourceNotFoundException()
    {
    }

    public ResourceNotFoundException(string message) : base(message)
    {
    }

    public ResourceNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    public ResourceNotFoundException(string resourceName, string resource) : base(
        $"Resource {resourceName} (resourceId={resource}) can not found")
    {
    }
}
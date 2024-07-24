namespace c6_boss_web_framework.framework.exceptions;

public class MethodNotAllowedException : Exception
{
    public MethodNotAllowedException()
    {
    }
    
    
    public MethodNotAllowedException(string httpMethod, string path) : base($"The method {httpMethod} is not allowed on {path}") {}
}
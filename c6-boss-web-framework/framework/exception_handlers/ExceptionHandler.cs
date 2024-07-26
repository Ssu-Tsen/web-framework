using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using c6_boss_web_framework.framework.exceptions;

namespace c6_boss_web_framework.framework.exception_handlers;

public class ExceptionHandlerChain(List<ExceptionHandler> handlers)
{
    public void HandleException(Exception e, HttpListenerResponse response)
    {
        foreach (var handler in handlers.Where(handler => handler.IsResponsible(e)))
        {
            handler.HandleException(e, response);
            break;
        }
    }
}

public abstract class ExceptionHandler
{
    public abstract bool IsResponsible(Exception e);

    public async void HandleException(Exception e, HttpListenerResponse response)
    {
        response.StatusCode = (int)GetStatusCode();
        response.ContentType = "plain/text";
        response.ContentEncoding = Encoding.UTF8;
        var errorBytes = Encoding.UTF8.GetBytes(e.Message);
        response.ContentLength64 = errorBytes.Length;
        await using var output = response.OutputStream;
        await output.WriteAsync(errorBytes);
    }

    protected virtual HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.InternalServerError;
    }
}
internal class NotFoundExceptionHandler : ExceptionHandler
{
    private const HttpStatusCode StatusCode = HttpStatusCode.NotFound;

    public override bool IsResponsible(Exception e)
    {
        return e is ResourceNotFoundException;
    }

    protected override HttpStatusCode GetStatusCode()
    {
        return StatusCode;
    }
}

internal class BadRequestExceptionHandler : ExceptionHandler
{
    private const HttpStatusCode StatusCode = HttpStatusCode.BadRequest;

    public override bool IsResponsible(Exception e)
    {
        return e is BadRequestException or ArgumentException or AuthenticationException;
    }

    protected override HttpStatusCode GetStatusCode()
    {
        return StatusCode;
    }
}

internal class UnauthorizedExceptionHandler : ExceptionHandler
{
    private const HttpStatusCode StatusCode = HttpStatusCode.Unauthorized;

    public override bool IsResponsible(Exception e)
    {
        return e is UnauthorizedException;
    }

    protected override HttpStatusCode GetStatusCode()
    {
        return StatusCode;
    }
}

internal class OperationForbiddenExceptionHandler : ExceptionHandler
{
    private const HttpStatusCode StatusCode = HttpStatusCode.Forbidden;

    public override bool IsResponsible(Exception e)
    {
        return e is OperationForbiddenException;
    }

    protected override HttpStatusCode GetStatusCode()
    {
        return StatusCode;
    }
}

internal class UnsupportedDeserializationTypeExceptionHandler : ExceptionHandler
{
    private const HttpStatusCode StatusCode = HttpStatusCode.InternalServerError;

    public override bool IsResponsible(Exception e)
    {
        return e is UnsupportedSerializationTypeException;
    }

    protected override HttpStatusCode GetStatusCode()
    {
        return StatusCode;
    }
}

internal class MethodNotAllowedExceptionHandler : ExceptionHandler
{
    private const HttpStatusCode StatusCode = HttpStatusCode.MethodNotAllowed;

    public override bool IsResponsible(Exception e)
    {
        return e is MethodNotAllowedException;
    }

    protected override HttpStatusCode GetStatusCode()
    {
        return StatusCode;
    }
}
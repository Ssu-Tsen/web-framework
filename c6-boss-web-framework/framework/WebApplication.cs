using System.Net;
using System.Text;
using c6_boss_web_framework.app.data_structure;
using c6_boss_web_framework.framework.exception_handlers;
using c6_boss_web_framework.framework.exceptions;
using c6_boss_web_framework.framework.http_body_serializers;
using c6_boss_web_framework.framework.ioc;
using c6_boss_web_framework.framework.requests;

namespace c6_boss_web_framework.framework;

public class WebApplication
{
    private readonly int _port;
    private readonly HttpListener _listener;
    private readonly HttpBodySerializerChain _serializer;
    private readonly RadixTree _router;
    private readonly ExceptionHandlerChain? _exceptionHandler;
    private readonly List<Type> _registeredTypes;
    private readonly List<Type> _controllerTypes;
    private readonly IIoCContainer _ioCContainer;

    public WebApplication(int port, HttpListener listener, RadixTree router,
        HttpBodySerializerChain serializer,
        ExceptionHandlerChain? exceptionHandler, List<Type> registeredTypes, List<Type> controllerTypes,
        IIoCContainer ioCContainer)
    {
        _port = port;
        _listener = listener;
        _serializer = serializer;
        _router = router;
        _exceptionHandler = exceptionHandler;
        _registeredTypes = registeredTypes;
        _controllerTypes = controllerTypes;
        _ioCContainer = ioCContainer;
        _listener.Prefixes.Add($"http://localhost:{_port}/");
        HttpRequest.SetHttpBodySerializer(_serializer);

        RegisterTypes(registeredTypes);
        RegisterControllers(controllerTypes);
    }

    private void RegisterTypes(List<Type> types)
    {
        foreach (var type in types)
        {
            _ioCContainer.Register(type);
        }
    }

    private void RegisterControllers(List<Type> types)
    {
        foreach (var type in types)
        {
            if (!typeof(Controller).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type {type.Name} is not a subclass of Controller");
            }
            _ioCContainer.Register(type);
        }

        _controllerTypes.Select(type => _ioCContainer.GetInstance<Controller>(type))
            .ToList()
            .ForEach(controller => controller?.Routes(_router));
    }

    public void AddSerializerPlugin(HttpBodySerializer serializer)
    {
        _serializer.AddSerializer(serializer);
    }

    public async Task Start()
    {
        _listener.Start();
        Console.WriteLine($"HTTP 伺服器已啟動，正在監聽");

        while (true)
        {
            HttpListenerContext context = await _listener.GetContextAsync();
            if (context.Request.Url == null)
            {
                Console.WriteLine("Request URL not found.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                continue;
            }

            HandleHttpRequest(context);
        }
    }

    private async void HandleHttpRequest(HttpListenerContext context)
    {
        var requestPath = context.Request.Url.AbsolutePath;
        var httpMethod = context.Request.HttpMethod;
        try
        {
            var handlerMethod = _router.Search(requestPath, httpMethod, out var pathVariables);
            var responseEntity = await handlerMethod.Invoke(context.Request, pathVariables);
            await Respond(responseEntity, context.Response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _exceptionHandler?.HandleException(e, context.Response);
        }
    }

    private async Task Respond(IResponseEntity responseEntity, HttpListenerResponse httpListenerResponse)
    {
        httpListenerResponse.Headers.Add(HttpResponseHeader.ContentType, responseEntity.ContentType);
        httpListenerResponse.Headers.Add(HttpResponseHeader.ContentEncoding, responseEntity.ContentEncoding);
        foreach (var (key, value) in responseEntity.Headers)
        {
            httpListenerResponse.Headers.Add(key, value);
        }

        httpListenerResponse.StatusCode = (int)responseEntity.StatusCode;
        var body = _serializer.Serialize(responseEntity.GetBody(), responseEntity.ContentType);
        httpListenerResponse.ContentLength64 = Encoding.UTF8.GetByteCount(body);
        await using var output = httpListenerResponse.OutputStream;
        await output.WriteAsync(Encoding.UTF8.GetBytes(body).AsMemory(0, body.Length));
    }

    public class Builder
    {
        private int _port;
        private IIoCContainer _ioc;
        private readonly List<HttpBodySerializer> _builtinSerializers = [new HttpBodyXmlSerializer()];

        private readonly List<ExceptionHandler> _builtinExceptionHandlers =
            [new NotFoundExceptionHandler(), new BadRequestExceptionHandler(), new UnauthorizedExceptionHandler()];

        private readonly List<HttpBodySerializer> _serializers = [];
        private readonly List<ExceptionHandler> _exceptionHandlers = [];
        private readonly List<Type> _controllerTypes = [];
        private readonly List<Type> _registeredTypes = [];

        public Builder Port(int port)
        {
            _port = port;
            return this;
        }

        public Builder Controllers(params Type[] types)
        {
            _controllerTypes.AddRange(types);
            return this;
        }

        public Builder IoC(IIoCContainer container)
        {
            _ioc = container;
            return this;
        }

        public Builder RegisterTypes(params Type[] types)
        {
            _registeredTypes.AddRange(types);
            return this;
        }

        public Builder Serializer(HttpBodySerializer serializer)
        {
            _serializers.Add(serializer);
            return this;
        }

        public Builder ExceptionHandler(ExceptionHandler exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
            return this;
        }

        public WebApplication Build()
        {
            HttpListener listener = new HttpListener();
            _builtinSerializers.AddRange(_serializers);
            _builtinExceptionHandlers.AddRange(_exceptionHandlers);

            return new WebApplication(_port, listener,
                new RadixTree(), new HttpBodySerializerChain(_builtinSerializers),
                new ExceptionHandlerChain(_builtinExceptionHandlers), _registeredTypes,
                _controllerTypes, _ioc);
        }
    }
}
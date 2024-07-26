using System.Net;
using c6_boss_web_framework.framework.requests;

namespace c6_boss_web_framework.framework.routers;

public interface IRouter
{
    Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> Search(string path,
        string httpMethod,
        out Dictionary<string, string> pathVariables);

    void Get(string path,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handlerMethod);

    void Post(string path,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handlerMethod);

    void Patch(string path,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handlerMethod);

    void Insert(string path, string httpMethod,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handler);
}
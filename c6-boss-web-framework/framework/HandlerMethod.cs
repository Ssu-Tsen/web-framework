using System.Net;

namespace c6_boss_web_framework;

public interface IHandlerMethod // router 的路由對象
{
    Task Handle(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, string> parameters);
}
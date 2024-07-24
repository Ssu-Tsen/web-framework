// See https://aka.ms/new-console-template for more information

// Console.WriteLine("Hello, World!");

using c6_boss_web_framework.app;
using c6_boss_web_framework.framework;
using c6_boss_web_framework.framework.http_body_serializers;
using c6_boss_web_framework.framework.ioc;

namespace c6_boss_web_framework;

internal static class Program
{
    // [POST] 註冊 http://localhost:8080/api/users/register  body --> 
    // http://localhost:8080/api/users --> body [{},{}]
    //  http://localhost:8080/api/users/{userid} --> body {}
    static async Task Main(string[] args)
    {
        var webApplication = new WebApplication.Builder()
            .Port(8080)
            .Controllers(typeof(UserController))
            .Serializer(new HttpBodyJsonSerializer())
            .RegisterTypes(typeof(UserSystem))
            .IoC(new DefaultIoCContainer())
            .Build();

        await webApplication.Start();

        // // 定義伺服器要監聽的 URL (包括指定的端口號)
        // string url = "http://localhost:8080/";
        // listener.Prefixes.Add(url);
        //
        //
        // // 開始監聽
        // listener.Start();
        // Console.WriteLine($"HTTP 伺服器已啟動，正在監聽 {url}");
        //
        // // 伺服器運行循環
        // while (true)
        // {
        //     // 等待一個 HTTP 請求
        //     HttpListenerContext context = await listener.GetContextAsync();
        //     HttpListenerRequest request = context.Request;
        //     // 準備 HTTP 回應
        //     HttpListenerResponse response = context.Response;
        //     string responseString = "<html><body>Hello, World!</body></html>";
        //     byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        //     response.ContentLength64 = buffer.Length;
        //     
        //     // 寫入回應內容
        //     using (Stream output = response.OutputStream)
        //     {
        //         output.Write(buffer, 0, buffer.Length);
        //     }
        //     
        //     Console.WriteLine("已處理請求： " + request.RawUrl);
        // }

        // 停止伺服器
        // listener.Stop();
    }
}
using System.Net;
using c6_boss_web_framework.framework.exceptions;
using c6_boss_web_framework.framework.requests;

namespace c6_boss_web_framework.framework.routers;

public class RadixTreeNode(string edgeLabel = "", bool isPathVariable = false)
{
    public string EdgeLabel { get; } = edgeLabel;
    public Dictionary<string, RadixTreeNode> Children { get; } = new();

    public Dictionary<string, Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>>>
        Handlers { get; } = new(); // Key: HTTP Method, Value: Handler

    public bool IsPathVariable { get; } = isPathVariable;
}

public class RadixTreeRouter : IRouter
{
    private readonly RadixTreeNode _root = new();
    private static readonly char[] Separator = ['/'];

    public void Get(string path,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handlerMethod)
    {
        Insert(path, "GET", handlerMethod);
    }
    
    public void Post(string path,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handlerMethod)
    {
        Insert(path, "POST", handlerMethod);
    }
    
    public void Patch(string path,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handlerMethod)
    {
        Insert(path, "PATCH", handlerMethod);
    }
    
    public void Insert(string path, string httpMethod,
        Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> handler)
    {
        var currentNode = _root;
        var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in segments)
        {
            var isPathVariable = segment.StartsWith('{') && segment.EndsWith('}');
            // TODO. check on this: var label = isPathVariable ? segment.Trim('{', '}') : segment;
            var label = segment;
            if (!currentNode.Children.TryGetValue(label, out var nextNode))
            {
                nextNode = new RadixTreeNode(label, isPathVariable);
                currentNode.Children[isPathVariable ? "{}" : label] = nextNode;
            }

            currentNode = nextNode;
        }

        currentNode.Handlers[httpMethod] = handler;
    }

    public Func<HttpListenerRequest, Dictionary<string, string>, Task<IResponseEntity>> Search(string path,
        string httpMethod,
        out Dictionary<string, string> pathVariables)
    {
        var currentNode = _root;
        var segments = path.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        pathVariables = new Dictionary<string, string>();

        foreach (var segment in segments)
        {
            if (currentNode.Children.TryGetValue(segment, out var nextNode))
            {
                currentNode = nextNode;
            }
            else if (currentNode.Children.TryGetValue("{}", out nextNode))
            {
                if (!nextNode.IsPathVariable)
                {
                    throw new Exception("Node should be path variable.");
                }

                pathVariables[nextNode.EdgeLabel.Trim('{', '}')] = segment;
                currentNode = nextNode;
            }
            else
            {
                throw new ResourceNotFoundException("Handler method", $"[{httpMethod}]{path}");
            }
        }

        if (!currentNode.Handlers.TryGetValue(httpMethod, out var handler))
        {
            throw new MethodNotAllowedException(httpMethod, path);
        }

        return handler;
    }
}
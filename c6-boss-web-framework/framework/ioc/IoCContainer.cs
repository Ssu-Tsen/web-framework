using System.Reflection;

namespace c6_boss_web_framework.framework.ioc;

public interface IIoCContainer
{
    void Register(Type type);

    void Register(InstanceDef instanceDef);

    void RegisterSingleton(object obj);

    T? GetInstance<T>(Type type);
}

public class DefaultIoCContainer : IIoCContainer
{
    private readonly Dictionary<Type, InstanceDef> _registered = new();

    public void Register(Type type)
    {
        _registered[type] = new InstanceDef(type, Scope.Singleton());
    }

    public void Register(InstanceDef instanceDef)
    {
        _registered[instanceDef.Type] = instanceDef;
    }

    public void RegisterSingleton(object obj)
    {
        if (_registered.ContainsKey(obj.GetType()))
        {
            // TODO: DuplicateInstanceTypeException
            throw new Exception($"Duplicate instance with type {obj.GetType()}");
        }

        var instanceDef = new InstanceDef(obj.GetType(), Scope.Singleton());
        _registered[obj.GetType()] = instanceDef;
    }

    public T? GetInstance<T>(Type type)
    {
        // var type = typeof(T);
        var assignableTypes = FindAssignableRegisteredTypes(type);
        if (assignableTypes.Count == 0)
        {
            // TODO: InstanceTypeNotFoundException
            throw new Exception("Instance type not found.");
        }

        if (assignableTypes.Count > 1)
        {
            // TODO: AmbiguousTypeException
            throw new Exception("Ambiguous type.");
        }

        var assignableType = assignableTypes[0];
        return ComputeIfAbsent(assignableType, Instantiate<T>);
    }

    private List<Type> FindAssignableRegisteredTypes(Type type)
    {
        return _registered.Keys.Where(type.IsAssignableFrom).ToList();
    }

    private T? Instantiate<T>(Type type)
    {
        var constructors = type.GetConstructors();
        try
        {
            if (constructors.Length == 0)
            {
                return (T)Activator.CreateInstance(type)!;
            }

            var constructor = constructors[0];
            var parameters = constructor.GetParameters()
                .Select(p => ComputeIfAbsent(p.ParameterType, GetInstance<object>))
                .ToArray();
            return (T?)constructor.Invoke(parameters);
        }
        catch (Exception e) when (e is TargetInvocationException or InvalidOperationException)
        {
            Console.WriteLine(e);
            // TODO: IllegalStateException
            throw;
        }
    }

    private T? ComputeIfAbsent<T>(Type type, Func<Type, T?> compute)
    {
        if (!_registered.TryGetValue(type, out var instanceDef))
        {
            return compute(type);
        }

        var scope = instanceDef.Scope;
        return scope.Get(() => compute(type));
    }
}
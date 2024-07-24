namespace c6_boss_web_framework.framework.ioc;

public interface IScope
{
    T Get<T>(Func<T> factory);
}

public class Scope
{
    public static IScope Prototype()
    {
        return new PrototypeScope();
    }

    public static IScope Singleton()
    {
        return new SingletonScope();
    }
    
}

public class SingletonScope : IScope
{
    private object? _instance;

    private object? Instance
    {
        get => _instance;
        set => _instance = value ?? throw new ArgumentNullException(nameof(value));
    }

    public T Get<T>(Func<T> factory)
    {
        Instance ??= factory();
        return (T)Instance!;
    }
}

public class PrototypeScope : IScope
{
    public T Get<T>(Func<T> factory)
    {
        return factory();
    }
}
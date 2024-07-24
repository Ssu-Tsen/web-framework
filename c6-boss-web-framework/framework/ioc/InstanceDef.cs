namespace c6_boss_web_framework.framework.ioc;

public class InstanceDef(Type type, IScope scope)
{
    public Type Type { get; } = type;
    public IScope Scope { get; } = scope;
}
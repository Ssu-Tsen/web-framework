using c6_boss_web_framework.framework.routers;

namespace c6_boss_web_framework.framework;

public abstract class Controller
{
    public abstract void Routes(IRouter router);
}
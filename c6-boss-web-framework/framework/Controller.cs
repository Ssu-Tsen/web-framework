using c6_boss_web_framework.app.data_structure;

namespace c6_boss_web_framework.framework;

public abstract class Controller
{
    public abstract void Routes(RadixTree router);
}
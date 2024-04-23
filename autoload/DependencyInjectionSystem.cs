using AssetManager.caches;
using AssetManager.util;
using Godot;
using SimpleInjector;
using System;
using Util.communication.commands;
using Util.json;
using Container = SimpleInjector.Container;

namespace AssetManager.autoload;

public partial class DependencyInjectionSystem : Node, IDependencyInjectionSystem
{
    public static DependencyInjectionSystem instance;
    private Container container;
    public override void _EnterTree()
    {
        base._EnterTree();
        instance = this;
        container = new Container();
        registerServices();
    }

    public object Resolve(Type type) => container.GetInstance(type);

    private void registerServices()
    {
        container.Register<ICommandManager, CommandManager>(Lifestyle.Singleton);
        var conf = Config.load<ConfigGeneral>();
        container.Register(() => conf, Lifestyle.Singleton);
        container.Register<Explorer>(Lifestyle.Singleton);
        container.Register<Pearls>(Lifestyle.Singleton);

        var ex = container.GetInstance<Explorer>();
    }


}

public static class Inject
{
    public static T Get<T>()
    {
        return (T) DependencyInjectionSystem.instance.Resolve(typeof(T));
    }
}
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
    private Container container;
    public override void _EnterTree()
    {
        base._EnterTree();
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

        var ex = container.GetInstance<Explorer>();

    }

}

public static class Inject
{

}

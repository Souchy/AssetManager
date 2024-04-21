using AssetManager.util;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.json;
using static AssetManager.autoload.DependencyInjectionSystem;

namespace AssetManager.autoload;

public partial class Main : Node
{
    public static ConfigGeneral ConfigGeneral { get; } = Inject.Get<ConfigGeneral>();

    public override void _EnterTree()
    {
    }

}

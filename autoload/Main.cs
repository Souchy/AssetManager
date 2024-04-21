using AssetManager.util;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.json;

namespace AssetManager.autoload;

public partial class Main : Node
{
    //[Inject]
    //public static ConfigGeneral ConfigGeneral { get; set; }

    public override void _EnterTree()
    {
        //this.Inject();
    }

}

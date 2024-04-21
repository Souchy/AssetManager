using AssetManager.loaders;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;
using Util.json;

namespace AssetManager;

internal class Pearls
{
    public static Pearls Instance { get; private set; }

    public Material DefaultMaterial { get; set; }

    public Godot.Collections.Dictionary<string, Array<item>> ItemsPerDirectory = new();


    private ConfigGeneral config { get; }

    public Pearls(ConfigGeneral config)
    {
        if(Instance != null) throw new Exception("Instantiated Pearls twice");
        Instance = this;
        EventBus.centralBus.subscribe(this);
        this.config = config;
    }

    public Array<item> CurrentItems
    {
        get => ItemsPerDirectory.ContainsKey(config.CurrentDirectory) ? ItemsPerDirectory[config.CurrentDirectory] : null;
        set => ItemsPerDirectory[config.CurrentDirectory] = value;
    }

}

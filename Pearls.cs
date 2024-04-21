using AssetManager.loaders;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;

namespace AssetManager;

internal class Pearls
{

    public static Material defaultMaterial {  get; set; }

    public static Godot.Collections.Dictionary<string, Array<item>> ItemsPerDirectory = new();

    private static readonly Pearls singleton = new();
    private Pearls()
    {
        EventBus.centralBus.subscribe(this);
    }

    //[Subscribe(Loaders.EventLoad)] / Problem: when we publish this event, we do so from Loaders which has Array<Node> instead of items. Need to makeItem in FlowView first
    //private void onLoad(Array<item> items)
    //{
    //    ItemsPerDirectory[Explorer.CurrentDirectory].AddRange(items);
    //}

}

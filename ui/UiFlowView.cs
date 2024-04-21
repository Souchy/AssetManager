using Godot;
using Godot.Sharp.Extras;
using System;
using static Godot.HttpRequest;
using System.Diagnostics.Metrics;
using Godot.Collections;
using System.Linq;
using AssetManager;
using Util.communication.events;
using AssetManager.loaders;
using System.IO;

public partial class UiFlowView : VBoxContainer
{
    public const string EventVisible = "ui.flow.vvisible";

    public static UiFlowView Instance;

    [NodePath]
    public HFlowContainer FlowItems { get; set; }

    //public PackedScene ItemFolderScene { get; set; }
    public PackedScene ItemTextureScene { get; set; }
    public PackedScene Item3DScene { get; set; }

    public int FlowCount => this.FlowItems.GetChildren().Count;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        Instance = this;

        FlowItems.RemoveAndQueueFreeChildren();
        //ItemFolderScene = GD.Load<PackedScene>("res://items/item_folder.tscn");
        ItemTextureScene = GD.Load<PackedScene>("res://items/item_texture.tscn");
        Item3DScene = GD.Load<PackedScene>("res://items/item_3d.tscn");
    }

    public void clear()
    {
        FlowItems.RemoveAndQueueFreeChildren();
    }

    [Subscribe(EventVisible)]
    public void onVisible() => this.Visible = true;

    [Subscribe(Loaders.EventLoad)] // Subscribe sur une fonction générique? no way que ça marche
    public void fill<[MustBeVariant] T>(Array<T> nodes)
    {
        //foreach (var n in nodes)
        //    makeItem(n);
        var items = nodes.Select(n => makeItem(n));
        Pearls.ItemsPerDirectory[Explorer.CurrentDirectory].AddRange(items);
    }

    public void refill(Array<item> items)
    {
        foreach (var item in items)
            this.FlowItems.AddChild(item);
    }

    public item makeItem(object obj)
    {
        if (obj is Node3D node)
        {
            var control = Item3DScene.Instantiate<item_3d>();
            FlowItems.AddChild(control);
            control.SetLabelName(node.Name);
            control.SetMesh(obj as Node3D);
            control.SetMaterial(Pearls.defaultMaterial);
            return control;
        }
        else 
        if (obj is ItemFolder item)
        {
            FlowItems.AddChild(item);
            item.SetLabelName(item.dir.Name);
            return item;
        }
        throw new Exception("unexpected type");
    }

    public void _on_btn_hide_flow_pressed()
    {
        this.Visible = false;
    }

}

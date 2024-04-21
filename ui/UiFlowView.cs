using Godot;
using Godot.Sharp.Extras;
using System;
using Godot.Collections;
using System.Linq;
using AssetManager;
using Util.communication.events;
using AssetManager.loaders;
using AssetManager.autoload;
using AssetManager.util;

public partial class UiFlowView : VBoxContainer
{
    public const string EventVisible = "ui.flow.vvisible";

    public static UiFlowView Instance;

    //public PackedScene ItemFolderScene { get; set; }
    public PackedScene ItemTextureScene { get; set; }
    public PackedScene Item3DScene { get; set; }

    #region Nodes
    [NodePath]
    public HFlowContainer FlowItems { get; set; }
    #endregion

    #region Inject
    [Inject]
    public ConfigGeneral config {  get; set; }
    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
        this.OnReady();
        this.Inject();
        EventBus.centralBus.subscribe(this);

        FlowItems.RemoveAndQueueFreeChildren();
        //ItemFolderScene = GD.Load<PackedScene>("res://ui/items/item_folder.tscn");
        ItemTextureScene = GD.Load<PackedScene>("res://ui/items/item_texture.tscn");
        Item3DScene = GD.Load<PackedScene>("res://ui/items/item_3d.tscn");
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
        Pearls.Instance.CurrentItems.AddRange(items);
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
            control.SetMaterial(Pearls.Instance.DefaultMaterial);
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
    public void _on_btn_reload_pressed()
    {
        Pearls.Instance.CurrentItems = null;
        Explorer.Instance.select(Explorer.Instance.config.CurrentDirectory);
    }

}

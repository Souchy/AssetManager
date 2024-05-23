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
using System.Collections;
using System.Collections.Generic;
using AssetManager.caches;

/// <summary>
/// Also want list mode on top of picture mode
/// 
/// File explorer, but also Model/Asset explorer, could change mode
///     But maybe the asset explorer from data should be different
///     maybe it stas just the list in DbExplorer. But kinda want to be able to view the models that are in a collection.
///     Can we add a flow view to the Properties tab? it would be nice. 
///     Put the collection properties at the top, then render of list of models contained as a flow or a list (can change mode, remember)
/// Then the "main" flowview would just be the file explorer i guess?
/// 
/// When items are added to the flow view but come from different folders, 
///     hwe should put a header between them to separate the folders and say where they're from
///     
/// 
/// ---- 
/// 22 avril, idk what i wrote above
/// but I really want to see the flow divided in sections based on database collections or primary tags
/// ex: go into folder, show WALLS sections, FLOORS section, PROPS section, then UNTAGGED section for the remaining assets that are unorganized
/// then sections are accordeons, collapsable
/// </summary>
public partial class UiFlowView : VBoxContainer
{
    public const string EventVisible = "ui.flow.visible";

    public static UiFlowView Instance;


    #region Nodes
    [NodePath] public HFlowContainer FlowItems { get; set; }
    [NodePath] public HBoxContainer PathNavigatorContainer { get; set; }
    #endregion

    #region Inject
    [Inject] public ConfigGeneral config { get; set; }
    #endregion

    #region Properties 
    public item LastItemSelected { get; set; }
    public List<item> SelectedItems { get; set; }
    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        this.Inject();
        EventBus.centralBus.subscribe(this);
        PathNavigatorContainer.RemoveAndQueueFreeChildren();
        FlowItems.RemoveAndQueueFreeChildren();
    }

    public void SetNavigatorPath(string path)
    {
        PathNavigatorContainer.RemoveAndQueueFreeChildren();
        var folders = path.Split("\\").ToList();
        int start = Math.Max(0, folders.Count - 4);
        for(int i = start; i < folders.Count; i++) 
        {
            var btn = new Button();
            btn.Text = folders[i] + "/";
            PathNavigatorContainer.AddChild(btn);
            btn.Pressed += () =>
            {
                var fullpath = string.Join('/', folders[..(i + 1)]);
                Explorer.Instance.select(fullpath);
            };
        }
    }

    #region Selection
    public void Select(item i, bool selected)
    {
        LastItemSelected = i;
        LastItemSelected.Select(selected);
    }

    public void SelectShift(item to)
    {
        if (LastItemSelected == null)
        {
            to.Select(true);
            return;
        }
        bool started = false;
        foreach (var child in FlowItems.GetChildren())
        {
            if (started)
            {
                (child as item).Select(true);
            }
            if (child == to) break;
            if (child == LastItemSelected) started = true;
        }
    }

    public void UnselectItems()
    {
        foreach (var child in this.FlowItems.GetChildren())
        {
            var item = child as item;
            item.Select(false);
        }
    }
    #endregion

    #region Add items
    public IEnumerable<T> GetItems<T>() where T : item
    {
        return FlowItems.GetChildren().Where(c => c is T).Cast<T>();
    }
    public void AddItem(item item) => this.FlowItems.AddChild(item);
    public void AddItems(IEnumerable<item> items)
    {
        foreach (var item in items)
            AddItem(item);
    }

    public void MakeAndAddItems(IEnumerable<GodotObject> objects) //<[MustBeVariant] T>(Array<T> nodes)
    {
        var items = objects.Select(n => MakeAndAddItem(n));
        Pearls.Instance.CurrentItems.AddRange(items);
    }

    public item MakeAndAddItem(GodotObject obj)
    {
        if (obj is Node3D node)
        {
            var item = Pearls.Instance.Item3DScene.Instantiate<item_3d>();
            AddItem(item);
            item.SetLabelName(node.Name);
            item.SetMesh(node);
            item.SetMaterial(Pearls.Instance.DefaultMaterial);
            return item;
        }
        else
        if (obj is ImageTexture texture)
        {
            var item = Pearls.Instance.ItemTextureScene.Instantiate<ItemTexture>();
            AddItem(item);
            item.SetLabelName(texture.ResourceName);
            item.SetTexture(texture);
            return item;
        }
        else
        if (obj is ItemFolder item)
        {
            AddItem(item);
            item.SetLabelName(item.dir.Name);
            return item;
        }
        throw new Exception("unexpected type");
    }
    #endregion

    public void clear()
    {
        foreach(var child in FlowItems.GetChildren())
            FlowItems.RemoveChild(child);
    }

    [Subscribe(EventVisible)]
    public void OnVisible() => this.Visible = true;


    #region Signal Handlers
    public void _on_btn_hide_flow_pressed()
    {
        this.Visible = false;
    }

    public void _on_btn_reload_pressed()
    {
        Pearls.Instance.CurrentItems = null;
        Explorer.Instance.select(Explorer.Instance.config.CurrentDirectory, true);
    }
    #endregion
}

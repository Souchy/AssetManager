using AssetManager;
using AssetManager.loaders;
using Godot;
using Godot.Collections;
using Godot.Sharp.Extras;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Util.communication.events;
using static Godot.HttpRequest;

public partial class UiMain : PanelContainer
{
    #region Views 
    [NodePath]
    public UiTreeExplorer UiTreeExplorer { get; set; }
    [NodePath]
    public UiStatusBar UiStatusBar { get; set; }
    [NodePath]
    public UiMenuBar UiMenuBar {  get; set; }
    [NodePath]
    public UiProperties UiProperties { get; set; }
    [NodePath]
    public UiFlowView UiFlowView { get; set; }

    [NodePath]
    public PanelContainer Scene { get; set; }
    #endregion


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
    }

}

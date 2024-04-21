using AssetManager;
using Godot;
using Godot.Sharp.Extras;
using System;
using Util.communication.events;
//using System.Windows.fo

public partial class UiMenuBar : MenuBar
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
    }

    public void _on_btn_open_pressed()
    {
        var dialog = new FileDialog
        {
            CurrentDir = Explorer.CurrentDirectory,
            Access = FileDialog.AccessEnum.Filesystem,
            FileMode = FileDialog.FileModeEnum.OpenDir,
            UseNativeDialog = true
        };
        dialog.DirSelected += Explorer.select;
        this.AddChild(dialog);
        dialog.PopupCentered();
    }

    public void _on_btn_gltf_pressed()
    {
        EventBus.centralBus.publish(UiStatusBar.EventStatusHeader, "Converting fbx to gltf:");
        AssetManager.Util.FbxToGltf(Explorer.SelectedPath);
    }

    public void _on_btn_show_flow_pressed()
    {
        EventBus.centralBus.publish(UiFlowView.EventVisible);
    }
}

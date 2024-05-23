using AssetManager;
using AssetManager.autoload;
using AssetManager.caches;
using AssetManager.util;
using Godot;
using Godot.Sharp.Extras;
using System;
using Util.communication.events;
//using System.Windows.fo

public partial class UiMenuBar : MenuBar
{
    [Inject]
    public Explorer explorer { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        this.Inject();
        EventBus.centralBus.subscribe(this);
    }

    public void _on_btn_open_pressed()
    {
        var dialog = new FileDialog
        {
            RootSubfolder = explorer.config.CurrentWorkspace.Path,
            CurrentDir = explorer.config.CurrentDirectory,
            Access = FileDialog.AccessEnum.Filesystem,
            FileMode = FileDialog.FileModeEnum.OpenDir,
            UseNativeDialog = true
        };
        dialog.DirSelected += explorer.select;
        this.AddChild(dialog);
        dialog.PopupCentered();
    }

    public void _on_btn_gltf_pressed()
    {
        EventBus.centralBus.publish(UiStatusBar.EventStatusHeader, "Converting fbx to gltf:");
        AssetManager.util.Util.FbxToGltf(explorer.config);
    }

    public void _on_btn_export_mesh_pressed()
    {
        EventBus.centralBus.publish(UiStatusBar.EventStatusHeader, "Exporting meshes:");
        EventBus.centralBus.publish(UiStatusBar.EventStatusValue, "0/0");
        AssetManager.util.Util.ExportMeshes();
    }

    public void _on_btn_show_flow_pressed()
    {
        EventBus.centralBus.publish(UiFlowView.EventVisible);
    }

    public void _on_btn_settings_pressed()
    {

    }

}

using AssetManager;
using AssetManager.autoload;
using AssetManager.db;
using AssetManager.loaders;
using Godot;
using Godot.Sharp.Extras;
using System;
using System.IO;
using Util.communication.events;
using Util.json;

public partial class UiTreeExplorer : VBoxContainer
{

    [NodePath]
    public Tree TreeItems { get; set; }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        TreeItems.RemoveAndQueueFreeChildren();
    }

    public void _on_btn_create_pack_from_synty_source_pressed()
    {
        var dialog = new FileDialog
        {
            CurrentDir = Main.ConfigGeneral.CurrentDirectory,
            Access = FileDialog.AccessEnum.Filesystem,
            FileMode = FileDialog.FileModeEnum.OpenDir,
            UseNativeDialog = true
        };
        dialog.DirSelected += createPack;
        this.AddChild(dialog);
        dialog.PopupCentered();
    }

    private void createPack(string path)
    {
        SyntyPack pack = new SyntyPack();

        var matdir = new DirectoryInfo(path + "/Textures/Alts/");
        if (matdir.Exists)
        {
            var texs = matdir.GetFiles();
            if (texs.Length > 0)
                Pearls.Instance.DefaultMaterial = MaterialLoader.loadMaterial(matdir.GetFiles()[0]);
        }
    }

}

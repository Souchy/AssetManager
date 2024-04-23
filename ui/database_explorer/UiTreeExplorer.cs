using AssetManager.autoload;
using AssetManager.caches;
using AssetManager.db;
using AssetManager.loaders;
using Godot;
using Godot.Sharp.Extras;
using System;
using System.IO;
using System.Linq;
using Util.communication.events;
using Util.json;

public partial class UiTreeExplorer : VBoxContainer
{
    #region Nodes
    [NodePath] public Tree TreeItems { get; set; }
    [NodePath] public Button BtnGenerateMaterialsFromTextures { get; set; }
    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        TreeItems.RemoveAndQueueFreeChildren();
        BtnGenerateMaterialsFromTextures.Pressed += BtnGenerateMaterialsFromTextures_Pressed;
    }

    private void BtnGenerateMaterialsFromTextures_Pressed()
    {
        var items = UiFlowView.Instance.FlowItems.GetChildren().Cast<item>();

        MaterialCollection collection = new();
        Texture2D emission = null;
        foreach (var item in items.Where(i => i.IsSelected && i is ItemTexture).Cast<ItemTexture>())
        {
            var albedoPath = item.TextureRect.Texture.ResourcePath;
            emission ??= MaterialLoader.GetAssociatedEmissionTexture(albedoPath);
            var mat = MaterialLoader.CreateMaterial(item.TextureRect.Texture, emission);
            collection.Materials.Add(mat);
            //Pearls.Instance.Assets.Add("material_path.tres", mat); // can only add mat if we save it to a .res file before
            ResourceSaver.Save(collection, $"{item.TextureRect.Name}.mat");
        }
        ResourceSaver.Save(collection, $"{collection.GetHashCode()}.matcol");
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

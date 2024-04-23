using AssetManager.autoload;
using AssetManager.caches;
using AssetManager.db;
using AssetManager.loaders;
using Godot;
using Godot.Collections;
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
    [NodePath] public Button BtnRefresh { get; set; }
    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        TreeItems.RemoveAndQueueFreeChildren();
        BtnGenerateMaterialsFromTextures.Pressed += BtnGenerateMaterialsFromTextures_Pressed;
        BtnRefresh.Pressed += Refresh;

        // Experiment
        {
            var pack = new SyntyPack();
            pack.NameOrId = "Test Pack";
            pack.MaterialCollections.Add(new("Materials A"));
            pack.MaterialCollections.Add(new("Decals"));
            pack.MaterialCollections.Add(new("Walls"));
            Diamonds.Packs.Add(pack);
        }

        Refresh();
    }

    private readonly Color white = Color.Color8(255, 255, 255, 255);
    public void Refresh()
    {
        this.TreeItems.Clear();
        this.TreeItems.Columns = 1;
        this.TreeItems.SelectMode = Tree.SelectModeEnum.Single;
        this.TreeItems.ItemMouseSelected += TreeItems_ItemMouseSelected;
        this.TreeItems.ItemIconDoubleClicked += TreeItems_ItemIconDoubleClicked;

        // Files
        var itemFiles = this.TreeItems.CreateItem();
        itemFiles.SetIcon(0, Pearls.Instance.LoadInternal<Texture2D>("res://Assets/icons/folder.png"));
        itemFiles.SetIconMaxWidth(0, 32);
        itemFiles.SetText(0, "Files");
        itemFiles.SetMetadata(1, "Files");
        itemFiles.SetMetadata(0, new Array<Variant>() {
            "Files", "Files", new Variant()
        });
        itemFiles.SetCustomColor(0, white);

        // Packs
        foreach (var pack in Diamonds.Packs)
        {
            AddPackItem(pack);
        }
    }

    public void AddPackItem(SyntyPack pack)
    {
        var itemPack = this.TreeItems.CreateItem();
        itemPack.SetIcon(0, Pearls.Instance.LoadInternal<Texture2D>("res://Assets/icons/package.png"));
        itemPack.SetIconMaxWidth(0, 32);
        itemPack.SetText(0, pack.NameOrId);
        itemPack.SetMetadata(0, new Array<Variant>() {
            nameof(SyntyPack),
            pack.GetType().FullName,
            pack
        });
        itemPack.SetCustomColor(0, white);
        itemPack.SetCollapsedRecursive(true);

        var itemMeshes = this.TreeItems.CreateItem(itemPack);
        itemMeshes.SetIcon(0, Pearls.Instance.LoadInternal<Texture2D>("res://Assets/icons/3d.png"));
        itemMeshes.SetIconMaxWidth(0, 32);
        itemMeshes.SetText(0, nameof(SyntyPack.Meshes));
        itemMeshes.SetMetadata(0, new Array<Variant>() {
            nameof(SyntyPack.Meshes),
            pack.Meshes.GetType().FullName,
            pack.Meshes
        });
        itemMeshes.SetCustomColor(0, white);

        var itemMatCols = this.TreeItems.CreateItem(itemPack);
        itemMatCols.SetIcon(0, Pearls.Instance.LoadInternal<Texture2D>("res://Assets/icons/files.png"));
        itemMatCols.SetIconMaxWidth(0, 32);
        itemMatCols.SetText(0, nameof(SyntyPack.MaterialCollections));
        itemMatCols.SetMetadata(0, new Array<Variant>() {
            nameof(SyntyPack.MaterialCollections),
            pack.MaterialCollections.GetType().FullName,
            pack.MaterialCollections
        });
        itemMatCols.SetCustomColor(0, white);

        foreach (var mats in pack.MaterialCollections)
        {
            var itemMats = this.TreeItems.CreateItem(itemMatCols);
            itemMats.SetIcon(0, Pearls.Instance.LoadInternal<Texture2D>("res://Assets/icons/material.png"));
            itemMats.SetIconMaxWidth(0, 32);
            itemMats.SetText(0, mats.NameOrId);
            itemMats.SetMetadata(0, new Array<Variant>() {
                nameof(MaterialCollection),
                mats.GetType().FullName,
                mats
            });
            itemMats.SetCustomColor(0, white);
        }
    }

    private void TreeItems_ItemIconDoubleClicked()
    {
        // allow to rename ? 
    }

    private void TreeItems_ItemMouseSelected(Vector2 position, long mouseButtonIndex)
    {
        var item = this.TreeItems.GetSelected();
        var array = item.GetMetadata(0).As<Array<Variant>>();
        var name = (string) array[0];
        var type = (string) array[1];
        var data = array[2];

        // TODO show in Flow or UiProperties.Flow
        //      1. Separate treeitems as categories by sections (accordeons)
        //      2. Allow dragging from filesFlow to dataFlow    to add items to collection

        if (name == "Files")
        {
            // show files in Flow.
            // this was put here with the thought that other items are also shown in Flow,
            // but maybe PropertiesFlow takes responsibility
        }
        else
        if (type == typeof(SyntyPack).FullName)
        {
            // TODO show in Flow or UiProperties.Flow
        }
        else
        if (type == typeof(Array<Node3D>).FullName) // && name == nameof(SyntyPack.Meshes))
        {
            // TODO show in Flow or UiProperties.Flow
        }
        else
        if (type == typeof(Array<MaterialCollection>).FullName) // && name == nameof(SyntyPack.MaterialCollections))
        {
            // TODO show in Flow or UiProperties.Flow
        }
        else
        if (type == typeof(MaterialCollection).FullName) // && name == nameof(MaterialCollection))
        {
            // TODO show in Flow or UiProperties.Flow
        }

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

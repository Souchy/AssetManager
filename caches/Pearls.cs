using AssetManager.loaders;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;
using Util.json;

namespace AssetManager.caches;

/// <summary>
/// Assets cache
/// </summary>
internal class Pearls
{
    public static Pearls Instance { get; private set; }

    #region Packed scenes
    public PackedScene ItemFolderScene { get; set; }
    public PackedScene ItemTextureScene { get; set; }
    public PackedScene Item3DScene { get; set; }
    #endregion

    #region Assets
    public Material DefaultMaterial { get; set; }
    public System.Collections.Generic.Dictionary<string, GodotObject> Assets { get; } = new();
    public Godot.Collections.Dictionary<string, Array<item>> ItemsPerDirectory = new();
    #endregion

    private ConfigGeneral Config { get; }

    public Pearls(ConfigGeneral config)
    {
        if (Instance != null) throw new Exception("Instantiated Pearls twice");
        Instance = this;
        this.Config = config;
        EventBus.centralBus.subscribe(this);

        ItemFolderScene = GD.Load<PackedScene>("res://ui/items/item_folder.tscn");
        ItemTextureScene = GD.Load<PackedScene>("res://ui/items/item_texture.tscn");
        Item3DScene = GD.Load<PackedScene>("res://ui/items/item_3d.tscn");
    }

    public Array<item> CurrentItems
    {
        get => ItemsPerDirectory.ContainsKey(Config.CurrentDirectory) ? ItemsPerDirectory[Config.CurrentDirectory] : null;
        set => ItemsPerDirectory[Config.CurrentDirectory] = value;
    }

    #region Getters
    public T LoadInternal<T>(string internalPath) where T : GodotObject
    {
        if (Assets.TryGetValue(internalPath, out GodotObject value))
        {
            return value as T;
        }
        var t = GD.Load<T>(internalPath);
        return t;
    }
    public Texture2D LoadTexture2D(string path)
    {
        if (Assets.TryGetValue(path, out GodotObject value))
        {
            return value as Texture2D;
        }
        var img = Image.LoadFromFile(path);
        var texture = ImageTexture.CreateFromImage(img);
        string name = path.Substring(path.LastIndexOf('\\'));
        texture.ResourceName = name;
        texture.ResourcePath = path;
        Assets[path] = texture;
        return texture;
    }

    private readonly GltfDocument gltfDocument = new();
    public Node3D LoadNode3D(string path)
    {
        if (Assets.TryGetValue(path, out GodotObject value))
        {
            return value as Node3D;
        }
        GltfState state = new();
        var err = gltfDocument.AppendFromFile(path, state);
        if (err != Error.Ok)
        {
            Console.WriteLine("error load3d: " + path);
            return null;
        }
        // FIXME: issue with using the same gltfDocument accross threads? error was something like writing on used memory
        var node = gltfDocument.GenerateScene(state);
        string name = path.Substring(path.LastIndexOf('\\'));
        node.Name = name;
        node.SceneFilePath = path;
        Assets[path] = node;
        return node as Node3D;
    }
    #endregion


}

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;

namespace AssetManager.loaders;
internal static class Loaders
{
    public const string EventLoad = "loaders.load.thread";
    public const string EventLoadCount = "loaders.load.count";

    public static PackedScene ItemFolderScene { get; set; }
    static Loaders()
    {
        ItemFolderScene ??= GD.Load<PackedScene>("res://ui/items/item_folder.tscn");
    }

    public static async void loadThreaded(string[] files)
    {
        // EVENT: clear flow + Pearls
        int threadSize = 3;
        int threadCount = 3; //(files.Length / threadSize) + 1;
        GodotThread[] threads = new GodotThread[threadCount];
        for (int t = 0; t < threads.Length; t++)
        {
            int index0 = t * threadSize;
            int index1 = Math.Min(index0 + threadSize, files.Length);
            string[] slice = files[index0..index1];

            var callable = Callable.From(() => Loaders.loadThings(slice));
            threads[t] = new();
            threads[t].Start(callable);
        }

        int counter = 0;
        foreach (var thread in threads)
        {
            var results = (Array<Node>) thread.WaitToFinish();
            counter += results.Count;
            //var items = results.Select(r => makeitem)
            //foreach (var n in nodes.Where(n => n != null))
            //    makeItem(n);

            // EVENT: add_flow + add_pearl + update_loaded_count
            UiFlowView.Instance.fill(results);
            //EventBus.centralBus.publish(EventLoad, results);
            EventBus.centralBus.publish(EventLoadCount, 0, files.Length);
            //UiFlowView.fill(results);
            //LblStatusValue.Text = results.Count + " / " + files.Length;
        }
    }

    public static Array<Node> loadThings(string[] files)
    {
        Array<Node> items = new();
        for (int i = 0; i < files.Length; i++)
        {
            if (i >= files.Length)
                break;
            var file = files[i];

            var ext = file[file.LastIndexOf(".")..].ToLower();
            Node item = null;
            if (isMesh(ext))
                item = load3d(file);
            if (isTexture(ext))
                item = loadTexture(file);
            if (item != null)
                items.Add(item);
        }
        return items;
    }

    public static Array<item> loadDirs(string[] dirs)
    {
        Array<item> items = new();
        foreach (var dir in dirs)
        {
            item item = loadDir(dir);
            if (item != null)
                items.Add(item);
        }
        return items;
    }
    public static ItemFolder loadDir(string dir)
    {
        var item = ItemFolderScene.Instantiate<ItemFolder>();
        item.dir = new DirectoryInfo(dir);
        return item;
    }

    private static GltfDocument gltfDocument = new();
    public static bool isMesh(string extension) => extension == ".glb" || extension == ".gltf";
    public static Node load3d(string file)
    {
        GltfState state = new();
        var err = gltfDocument.AppendFromFile(file, state);
        if (err != Error.Ok)
        {
            Console.WriteLine("error load3d: " + file);
            return null;
        }
        var node = gltfDocument.GenerateScene(state);
        string name = file.Substring(file.LastIndexOf('\\'));
        node.Name = name;
        return node;
    }


    public static bool isTexture(string extension) => extension == ".png";
    public static Node loadTexture(string file)
    {
        return null;
    }
}

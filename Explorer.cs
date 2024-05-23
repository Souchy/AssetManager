using AssetManager.autoload;
using AssetManager.caches;
using AssetManager.db;
using AssetManager.loaders;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;
using Util.json;

namespace AssetManager;

public class Explorer
{
    public static Explorer Instance { get; private set; }

    public ConfigGeneral config { get; }

    public Explorer(ConfigGeneral config)
    {
        if (Instance != null) throw new Exception("Instantiated Explorer twice");
        Instance = this;
        this.config = config;
        config.CurrentDirectory = "G://Assets/pack/HumbleBundle/Synty"; // Mansion;
    }

    public void select(string path) => select(path, false);
    public void select(string path, bool forceReload = false)
    {
        if (isDir(path))
        {
            config.CurrentDirectory = path;
            config.SelectedPath = path;
            config.save();
            // Refresh le FlowView
            onOpenedDirectory(forceReload);
        }
        else
        {

        }
        //File.
        //Directory.i
    }

    private bool isDir(string path)
    {
        return (File.GetAttributes(path) & FileAttributes.Directory) > 0;
    }

    private void onOpenedDirectory(bool forceReload = false)
    {
        AssetFolder.CurrentFolder = Config.load<AssetFolder>(config.CurrentDirectory + "/assetfolder.json");

        // Material stuff that will be removed in the future
        var matdir = new DirectoryInfo(config.CurrentDirectory + "/Textures/Alts/");
        if (matdir.Exists)
        {
            var texs = matdir.GetFiles();
            if (texs.Length > 0)
                Pearls.Instance.DefaultMaterial = MaterialLoader.loadMaterial(matdir.GetFiles()[0]);
        }

        // Clear items
        var oldItems = Pearls.Instance.CurrentItems;
        UiFlowView.Instance.clear();

        // Update navigation bar
        UiFlowView.Instance.SetNavigatorPath(config.CurrentDirectory);

        // Folder up
        ItemFolder folderUp = Pearls.Instance.ItemFolderScene.Instantiate<ItemFolder>();
        UiFlowView.Instance.AddItem(folderUp);
        folderUp.dir = new DirectoryInfo(config.CurrentDirectory).Parent;
        folderUp.SetLabelName("..");

        // Old items
        if (oldItems != null && !forceReload)
        {
            UiFlowView.Instance.AddItems(oldItems);
            EventBus.centralBus.publish(Loaders.EventLoadCount, oldItems.Count, oldItems.Count);
            return;
        }
        else
        if (forceReload)
        {
            foreach(var a in oldItems)
            {
                a.QueueFree();
                Pearls.Instance.Assets.Remove(a.Path);
            }

            Pearls.Instance.ItemsPerDirectory.Remove(config.CurrentDirectory);
        }

        // New items
        Pearls.Instance.CurrentItems = new();

        // load dirs
        var dirs = Directory.GetDirectories(config.SelectedPath, "");
        var dirItems = Loaders.loadDirs(dirs);
        UiFlowView.Instance.MakeAndAddItems(dirItems);
        Pearls.Instance.CurrentItems.AddRange(dirItems);

        // load files
        var files = Directory.GetFiles(config.SelectedPath, "");
        Loaders.loadThreaded(files);
        // clean up
        //if (oldItems != null)
        //    foreach (var olditem in oldItems)
        //        olditem.QueueFree();
    }

}

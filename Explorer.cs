using AssetManager.autoload;
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

    public void select(string path)
    {
        if (isDir(path))
        {
            config.CurrentDirectory = path;
            config.SelectedPath = path;
            // Refresh le FlowView
            onOpenedDirectory();
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

    private void onOpenedDirectory()
    {
        var matdir = new DirectoryInfo(config.CurrentDirectory + "/Textures/Alts/");
        if (matdir.Exists)
        {
            var texs = matdir.GetFiles();
            if (texs.Length > 0)
                Pearls.Instance.DefaultMaterial = MaterialLoader.loadMaterial(matdir.GetFiles()[0]);
        }
        var oldItems = Pearls.Instance.CurrentItems;
        if (oldItems != null)
        {
            UiFlowView.Instance.refill(oldItems);
            EventBus.centralBus.publish(Loaders.EventLoadCount, oldItems.Count, oldItems.Count);
            return;
        }

        UiFlowView.Instance.clear();
        Pearls.Instance.CurrentItems = new();

        // load dirs
        var dirs = Directory.GetDirectories(config.SelectedPath, "");
        var dirItems = Loaders.loadDirs(dirs);
        UiFlowView.Instance.fill(dirItems);
        Pearls.Instance.CurrentItems.AddRange(dirItems);

        // load files
        var files = Directory.GetFiles(config.SelectedPath, "");
        Loaders.loadThreaded(files);
        // clean up
        if (oldItems != null)
            foreach (var olditem in oldItems)
                olditem.QueueFree();
    }

}

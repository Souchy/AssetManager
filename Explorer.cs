using AssetManager.loaders;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.communication.events;

namespace AssetManager;

internal class Explorer
{
    #region Configuration
    public const string fbx2gltf = "G:/Downloads/FBX2glTF-windows-x86_64.exe";
    public const string GeneratedPath = "*__AS_GEN_GLTF";
    public const string Mansion = "G://Assets/pack/HumbleBundle/Synty/BUNDLE_1/POLYGON_Horror_CurrentDirectory_SourceFiles/SourceFiles";
    public string selectName = "FBX";
    #endregion

    #region Paths
    public static string CurrentDirectory { get; set; }
    public static string SelectedPath { get => SelectedPaths[0]; set => SelectedPaths = [value]; }
    public static string[] SelectedPaths { get; set; } = new string[1];
    #endregion

    static Explorer()
    {
        CurrentDirectory = "G://Assets/pack/HumbleBundle/Synty"; // Mansion;
    }

    public static void select(string path)
    {
        if (isDir(path))
        {
            CurrentDirectory = path;
            SelectedPath = path;
            // Refresh le FlowView
            onOpenedDirectory();
        }
        else
        {

        }
        //File.
        //Directory.i
    }

    private static bool isDir(string path)
    {
        return (File.GetAttributes(path) & FileAttributes.Directory) > 0;
    }

    private static void onOpenedDirectory()
    {
        var matdir = new DirectoryInfo(CurrentDirectory + "/Textures/Alts/");
        if(matdir.Exists)
        {
            var texs = matdir.GetFiles();
            if (texs.Length > 0)
                Pearls.defaultMaterial = MaterialLoader.loadMaterial(matdir.GetFiles()[0]);
        }

        if (Pearls.ItemsPerDirectory.TryGetValue(CurrentDirectory, out Array<item> items))
        {
            UiFlowView.Instance.refill(items);
            EventBus.centralBus.publish(Loaders.EventLoadCount, items.Count, items.Count);
            return;
        }

        UiFlowView.Instance.clear();
        var old = items; //Pearls.ItemsPerDirectory.ge[CurrentDirectory];
        Pearls.ItemsPerDirectory[CurrentDirectory] = new();


        // load dirs
        var dirs = Directory.GetDirectories(SelectedPath, "");
        var dirItems = Loaders.loadDirs(dirs);
        UiFlowView.Instance.fill(dirItems);
        Pearls.ItemsPerDirectory[CurrentDirectory].AddRange(dirItems);

        // load files
        var files = Directory.GetFiles(SelectedPath, "");
        Loaders.loadThreaded(files);
        foreach (var olditem in old)
            olditem.QueueFree();
    }

}

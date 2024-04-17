using Godot;
using Godot.Sharp.Extras;
using System;
using System.Diagnostics;
using System.IO;

public partial class main : PanelContainer
{
    #region Nodes
    [NodePath]
    public Tree TreeItems { get; set; }
    [NodePath]
    public HFlowContainer FlowItems { get; set; }
    [NodePath]
    public GridContainer GridProperties { get; set; }
    [NodePath]
    public PanelContainer Scene { get; set; }
    #endregion

    public const string fbx2gltf = "G:/Downloads/FBX2glTF-windows-x86_64.exe";
    public const string GeneratedPath = "GLTF_GENERATED_SM";
    public const string Mansion = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE #1\\POLYGON_Horror_Mansion_SourceFiles\\SourceFiles\\";
    public string SelectedPath { get; set; }
    public PackedScene ItemFolderScene { get; set; }
    public PackedScene ItemTextureScene { get; set; }
    public PackedScene Item3DScene { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();

        GridProperties.RemoveAndQueueFreeChildren();
        SelectedPath = Mansion + "FBX";
        Item3DScene = GD.Load<PackedScene>("res://items/item_3d.tscn");
        Item3DScene = GD.Load<PackedScene>("res://items/item_3d.tscn");
        Item3DScene = GD.Load<PackedScene>("res://items/item_3d.tscn");
    }

    public void _on_btn_open_pressed()
    {
        GD.Print("ON OPEN");
        //var dialog = new FileDialog();
        //dialog.CurrentDir = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE #1";
        ////dialog.CurrentPath = "";
        ////dialog.CurrentFile = "";
        ////dialog.CurrentScreen = "";
        //dialog.Show();
        //Viewport viewport = new Viewport();
        //File.


    }

    private void load3d()
    {
        //GltfDocument
        GltfDocument gltfDocument = new GltfDocument();
        //EditorSceneFormatImporterFbx importer = new EditorSceneFormatImporterFbx();
        //importer.

    }

    public void _on_btn_gltf_pressed()
    {
        GD.Print("ON GLTF");
        bool force = false;
        var selectedDir = new DirectoryInfo(SelectedPath);
        //var gltfDir = Directory.CreateDirectory(selectedDir.Parent.FullName + "/" + GeneratedPath);

        var directories = Directory.GetDirectories(SelectedPath);
        foreach(var dir in directories)
        {
            string output = dir.Replace($"\\{selectedDir.Name}\\", $"\\{GeneratedPath}\\");
            Directory.CreateDirectory(output);
        }

        var fbx_files = Directory.GetFiles(SelectedPath, "*.fbx", SearchOption.AllDirectories);
        for(int i = 0; i < 3; i++)
        {
            var input = fbx_files[i];
            string output = input.Replace($"\\{selectedDir.Name}\\", $"\\{GeneratedPath}\\") + ".glb";
            if(File.Exists(output) && !force)
                continue;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = fbx2gltf;
            psi.ArgumentList.Add("-b");
            psi.ArgumentList.Add("-i");
            psi.ArgumentList.Add("\"" + input + "\"");
            psi.ArgumentList.Add("-o");
            psi.ArgumentList.Add("\"" + output + "\"");
            Process.Start(psi);
            //Process.Start(fbx2gltf, $"-b -i {input} -o {output}");
        }
    }


}

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

    #region Configuration
    public const string fbx2gltf = "G:/Downloads/FBX2glTF-windows-x86_64.exe";
    public const string GeneratedPath = "GLTF_GENERATED_SM";
    public const string Mansion = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE_1\\POLYGON_Horror_Mansion_SourceFiles\\SourceFiles\\";
    #endregion

    public string SelectedPath { get; set; }
    public PackedScene ItemFolderScene { get; set; }
    public PackedScene ItemTextureScene { get; set; }
    public PackedScene Item3DScene { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();

        GridProperties.RemoveAndQueueFreeChildren();
        FlowItems.RemoveAndQueueFreeChildren();
        TreeItems.RemoveAndQueueFreeChildren();

        SelectedPath = Mansion + "FBX\\";
        ItemFolderScene = GD.Load<PackedScene>("res://items/item_folder.tscn");
        ItemTextureScene = GD.Load<PackedScene>("res://items/item_texture.tscn");
        Item3DScene = GD.Load<PackedScene>("res://items/item_3d.tscn");
    }

    public void _on_btn_open_pressed()
    {
        GD.Print("ON OPEN");
        SelectedPath = Mansion + GeneratedPath;
        FlowItems.RemoveAndQueueFreeChildren();

        var files = Directory.GetFiles(SelectedPath, "", SearchOption.AllDirectories);
        for (int i = 0; i < 5; i++)
        {
            var file = files[i];
            if(isMesh(file))
                load3d(file);
            if (isTexture(file))
                loadTexture(file);
        }
        //var dialog = new FileDialog();
        //dialog.CurrentDir = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE #1";
        ////dialog.CurrentPath = "";
        ////dialog.CurrentFile = "";
        ////dialog.CurrentScreen = "";
        //dialog.Show();
        //Viewport viewport = new Viewport();
        //File.


    }

    private bool isMesh(string file)
    {
        return file.EndsWith(".glb", StringComparison.InvariantCultureIgnoreCase) || file.EndsWith(".gltf", StringComparison.InvariantCultureIgnoreCase);
    }
    private void load3d(string file)
    {
        GltfDocument gltfDocument = new();
        GltfState state = new();
        var err = gltfDocument.AppendFromFile(file, state);
        if(err != Error.Ok)
        {
            Console.WriteLine("error load3d: " + file);
            return;
        }
        var node = (Node3D) gltfDocument.GenerateScene(state);
        var control = Item3DScene.Instantiate<item_3d>();
        //var oldmesh = control.GetNode("%MeshInstance3D");
        //oldmesh.ReplaceBy(node);
        //oldmesh.GetParent().AddChild(node);
        //oldmesh.QueueFree();
        FlowItems.AddChild(control);
        control.SetMesh(node);
    }

    private bool isTexture(string file)
        => file.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase);
    private void loadTexture(string file)
    {

    }

    public void _on_btn_gltf_pressed()
    {
        GD.Print("ON GLTF");
        bool force = false;
        var selectedDir = new DirectoryInfo(SelectedPath);
        //var gltfDir = Directory.CreateDirectory(selectedDir.Parent.FullName + "/" + GeneratedPath);

        var directories = Directory.GetDirectories(SelectedPath);
        foreach (var dir in directories)
        {
            string output = dir.Replace($"\\{selectedDir.Name}\\", $"\\{GeneratedPath}\\");
            Directory.CreateDirectory(output);
        }

        var fbx_files = Directory.GetFiles(SelectedPath, "*.fbx", SearchOption.AllDirectories);
        for (int i = 0; i < fbx_files.Length; i++)
        {
            var input = fbx_files[i];
            string output = input.Replace($"\\{selectedDir.Name}\\", $"\\{GeneratedPath}\\") + ".glb";
            if (File.Exists(output) && !force)
                continue;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = fbx2gltf;
            psi.ArgumentList.Add("-b");
            psi.ArgumentList.Add("-i");
            psi.ArgumentList.Add(input);
            psi.ArgumentList.Add("-o");
            psi.ArgumentList.Add(output);
            var process = Process.Start(psi);
            //process.WaitForExit();
            
            //string stdout = process.StandardOutput.ReadToEnd(); 
            //Process.Start(fbx2gltf, $"-b -i {input} -o {output}");
        }
    }


}

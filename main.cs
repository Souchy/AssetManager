using Godot;
using Godot.Collections;
using Godot.Sharp.Extras;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    [NodePath]
    public Label LblStatusHeader { get; set; }
    [NodePath]
    public Label LblStatusValue { get; set; }
    [NodePath]
    public Label LblFpsValue { get; set; }
    #endregion

    #region Configuration
    public string fbx2gltf = "G:/Downloads/FBX2glTF-windows-x86_64.exe";
    public string GeneratedPath = "*__AS_GEN_GLTF";
    public string Mansion = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE_1\\POLYGON_Horror_Mansion_SourceFiles\\SourceFiles\\";
    public string selectName = "FBX";
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
        LblStatusHeader.Text = "";
        LblStatusValue.Text = "";

        ItemFolderScene = GD.Load<PackedScene>("res://items/item_folder.tscn");
        ItemTextureScene = GD.Load<PackedScene>("res://items/item_texture.tscn");
        Item3DScene = GD.Load<PackedScene>("res://items/item_3d.tscn");
    }

    public override void _Process(double delta)
    {
        LblFpsValue.Text = (1.0 / delta).ToString();
    }

    public Material ourMaterial;
    public void _on_btn_open_pressed()
    {
        GD.Print("ON OPEN");
        var newPath = GeneratedPath.Replace("*", selectName);
        SelectedPath = Mansion + newPath;
        //var dialog = new FileDialog();
        //dialog.CurrentDir = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE #1";
        ////dialog.CurrentPath = "";
        ////dialog.CurrentFile = "";
        ////dialog.CurrentScreen = "";
        //dialog.Show();
        //Viewport viewport = new Viewport();
        //File.

        FlowItems.RemoveAndQueueFreeChildren();

        var matdir = new DirectoryInfo(Mansion + "Textures\\Alts\\");
        var texs = matdir.GetFiles();
        if (texs.Length > 0)
            ourMaterial = loadMaterial(matdir.GetFiles()[0]);

        var files = Directory.GetFiles(SelectedPath, "", SearchOption.AllDirectories);
        LblStatusHeader.Text = "Loading:";

        int threadSize = 100;
        int threadCount = (files.Length / threadSize) + 1;
        GodotThread[] threads = new GodotThread[threadCount]; //(files.Length / threadSize)];
        for (int t = 0; t < threads.Length; t++)
        {
            int index0 = t * threadSize;
            int index1 = Math.Min(index0 + threadSize, files.Length);
            string[] slice = files[index0..index1];

            var callable = Callable.From(() => loadThings(slice));
            threads[t] = new();
            threads[t].Start(callable);
        }

        int counter = 0;
        foreach (var thread in threads)
        {
            var results = (Array<Node>) thread.WaitToFinish();
            foreach (var n in results.Where(n => n != null))
            {
                counter++;
                makeItem(n);
                LblStatusValue.Text = counter + " / " + files.Length;
            }
        }
    }

    private Array<Node> loadThings(string[] files)
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

    private Material loadMaterial(FileInfo textureFile) //string texture)
    {
        var path = textureFile.FullName;
        // TODO load materials
        var mat = new StandardMaterial3D();
        var img = Image.LoadFromFile(path);
        var tex = ImageTexture.CreateFromImage(img);
        mat.AlbedoTexture = tex;

        var emi = path[..^5] + "Emissive.png";
        if (File.Exists(emi))
        {
            var imgemi = Image.LoadFromFile(emi);
            var texemi = ImageTexture.CreateFromImage(imgemi);
            mat.EmissionTexture = texemi;
        }
        return mat;
    }

    private bool isMesh(string extension) => extension == ".glb" || extension == ".gltf";
    private Node load3d(string file)
    {
        GltfDocument gltfDocument = new();
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

    public void makeItem(Node node)
    {
        if (node is Node3D)
        {
            var control = Item3DScene.Instantiate<item_3d>();
            FlowItems.AddChild(control);
            control.SetLabelName(node.Name);
            control.SetMesh(node as Node3D);
            control.SetMaterial(ourMaterial);
        }
    }

    private bool isTexture(string extension) => extension == ".png";
    private Node loadTexture(string file)
    {
        return null;
    }

    public void _on_btn_gltf_pressed()
    {
        SelectedPath = Mansion + selectName;
        bool force = false;

        GD.Print("ON GLTF");
        var selectedDir = new DirectoryInfo(SelectedPath);
        //var gltfDir = Directory.CreateDirectory(selectedDir.Parent.FullName + "/" + GeneratedPath);
        var newPath = GeneratedPath.Replace("*", selectedDir.Name);

        var fbx_files = Directory.GetFiles(SelectedPath, "*.fbx", SearchOption.AllDirectories);
        LblStatusHeader.Text = "Converting fbx to gltf:";
        for (int i = 0; i < fbx_files.Length; i++)
        {
            var input = fbx_files[i];
            string output = input.Replace($"\\{selectedDir.Name}\\", $"\\{newPath}\\").Replace(".fbx", ".glb");
            if (File.Exists(output) && !force)
            {
                LblStatusValue.Text = (i + 1) + " / " + fbx_files.Length;
                continue;
            }
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

            LblStatusValue.Text = (i + 1) + " / " + fbx_files.Length;
            //string stdout = process.StandardOutput.ReadToEnd(); 
            //Process.Start(fbx2gltf, $"-b -i {input} -o {output}");
        }
    }


}

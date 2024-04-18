using Godot;
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
        double threadSize = 3;
        List<List<item>> shardedItems = new();
        Task[] tasks = new Task[1]; //(int) Math.Ceiling(files.Length / threadSize)];
        for (int t = 0; t < tasks.Length; t++)
        {
            var tt = t;
            shardedItems.Add(new());
            //var task = new Task(() =>
            //{
                for (int i = tt; i < (tt + 1) * threadSize; i++)
                {
                    if (i >= files.Length)
                        return;
                    var file = files[i];
                    var ext = file[file.LastIndexOf(".")..].ToLower();
                    item item = null;
                    if (isMesh(ext))
                        item = load3d(file);
                    if (isTexture(ext))
                        item = loadTexture(file);
                    //var item = ext switch
                    //{
                    //    var a when isMesh(a) => load3d(file),
                    //    var b when isTexture(b) => loadTexture(file),
                    //    var c => throw new NotImplementedException()
                    //};
                    if (item != null)
                        shardedItems[t].Add(item);
                    //LblStatusValue.Text = (i + 1) + " / " + files.Length;
                }
            //});
            //tasks[tt] = task;
            //task.Start();
        }
        //Task.WaitAll(tasks);

        int counter = 0;
        foreach (var t in shardedItems)
        {
            foreach (var n in t)
            {
                counter++;
                FlowItems.AddChild(n);
                n.Initializer?.Invoke();
                LblStatusValue.Text = counter + " / " + files.Length;
                //FlowItems.CallThreadSafe(nameof(FlowItems.AddChild), n);
                //n.CallThreadSafe(nameof(item.initialize), n);
            }
        }
    }

    public void asdf()
    {

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
    private item_3d load3d(string file)
    {
        GltfDocument gltfDocument = new();
        GltfState state = new();
        var err = gltfDocument.AppendFromFile(file, state);
        if (err != Error.Ok)
        {
            Console.WriteLine("error load3d: " + file);
            return null;
        }
        var node = (Node3D) gltfDocument.GenerateScene(state);
        var control = Item3DScene.Instantiate<item_3d>();

        control.Initializer = () =>
        {
            control.SetMesh(node);
            control.SetName(file.Substring(file.LastIndexOf('\\')));
            control.SetMaterial(ourMaterial);
        };

        //new Dispatcher().
        //FlowItems.Call(nameof(FlowItems.AddChild), control);
        //FlowItems.AddChild(control);

        return control;
    }

    private bool isTexture(string extension) => extension == ".png";
    private item loadTexture(string file)
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

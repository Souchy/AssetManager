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
    public const string GeneratedPath = "*__AS_GEN_GLTF";
    public const string Mansion = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE_2\\POLYGON_FantasyRivals_Source_Files\\Source_Files\\";
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

        ItemFolderScene = GD.Load<PackedScene>("res://items/item_folder.tscn");
        ItemTextureScene = GD.Load<PackedScene>("res://items/item_texture.tscn");
        Item3DScene = GD.Load<PackedScene>("res://items/item_3d.tscn");
    }

    public Material ourMaterial;
    public void _on_btn_open_pressed()
    {
        GD.Print("ON OPEN");
        string selectName = "Characters";
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

        var texfol = "G:\\Assets\\pack\\HumbleBundle\\Synty\\BUNDLE_2\\POLYGON_FantasyRivals_Source_Files\\Source_Files\\Textures\\";
        ourMaterial = loadMaterial(texfol + "FantasyRivals_Texture_01_A.png");

        var files = Directory.GetFiles(SelectedPath, "", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            if (isMesh(file))
                load3d(file);
            if (isTexture(file))
                loadTexture(file);
        }
    }

    private Material loadMaterial(string texture)
    {
        // TODO load materials
        var mat = new StandardMaterial3D();
        var img = Image.LoadFromFile(texture);
        var tex = ImageTexture.CreateFromImage(img);
        mat.AlbedoTexture = tex;

        var emi = texture[..^5] + "Emissive.png";
        if(File.Exists(emi))
        {
            var imgemi = Image.LoadFromFile(emi);
            var texemi = ImageTexture.CreateFromImage(imgemi);
            mat.EmissionTexture = texemi;
        }
        return mat;
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
        if (err != Error.Ok)
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
        control.SetName(file.Substring(file.LastIndexOf('\\')));
        control.SetMaterial(ourMaterial);
    }

    private bool isTexture(string file)
        => file.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase);
    private void loadTexture(string file)
    {

    }

    public void _on_btn_gltf_pressed()
    {
        SelectedPath = Mansion + "Characters\\";
        bool force = false;

        GD.Print("ON GLTF");
        var selectedDir = new DirectoryInfo(SelectedPath);
        //var gltfDir = Directory.CreateDirectory(selectedDir.Parent.FullName + "/" + GeneratedPath);
        var newPath = GeneratedPath.Replace("*", selectedDir.Name);

        var fbx_files = Directory.GetFiles(SelectedPath, "*.fbx", SearchOption.AllDirectories);
        for (int i = 0; i < fbx_files.Length; i++)
        {
            var input = fbx_files[i];
            string output = input.Replace($"\\{selectedDir.Name}\\", $"\\{newPath}\\").Replace(".fbx", ".glb");
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

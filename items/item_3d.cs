using AssetManager;
using Godot;
using Godot.Collections;
using Godot.Sharp.Extras;
using System;
using System.Linq;

public partial class item_3d : item
{
    private bool hovering {  get; set; }

    [NodePath]
    public Node3D Model { get; set; }
    [NodePath]
    public SubViewport SubViewport { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.OnReady();
        //Initializer?.Invoke();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if(hovering)
        {
            //Model.Rotate(Vector3.Up, (float) (0.5 * delta));
            Model.RotateY((float)(3 * delta));
            //Model.GlobalRotate(Vector3.Up, (float)(0.5 * delta));
            //Model.RotationDegrees = new Vector3(0, 30, 0);
            //GD.Print("rotate");
        }
	}

    public void SetMesh(Node3D node)
    {
        Model.ReplaceBy(node);
        Model = node;
        var meshes = node.GetChildrenOfType<MeshInstance3D>();
        foreach (var mesh in meshes)
        {
            mesh.LodBias = 128;
        }
    }

    public void SetMaterial(Material material)
    {
        var meshes = Model.GetChildrenOfType<MeshInstance3D>();
        
        foreach(var mesh in meshes)
        {
            mesh.MaterialOverride = material;
        }
    }

    public void _on_mouse_entered()
    {
        hovering = true;
        SubViewport.RenderTargetClearMode = SubViewport.ClearMode.Always;
        SubViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
    }

    public void _on_mouse_exited()
    {
        hovering = false;
        Model.Rotation = new Vector3();
        SubViewport.RenderTargetClearMode = SubViewport.ClearMode.Once;
        SubViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
    }


}

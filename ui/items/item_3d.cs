using AssetManager;
using Godot;
using Godot.Collections;
using Godot.Sharp.Extras;
using System;
using System.Linq;
using Util.communication.events;

public partial class item_3d : item
{
    private bool hovering { get; set; }

    [NodePath]
    public Node3D Model { get; set; }
    [NodePath]
    public SubViewport SubViewport { get; set; }
    [NodePath]
    public Camera3D Camera3D { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        //this.GetParent().Connect("visible", Callable.From(_on_mouse_exited));
        _on_mouse_exited();
    }

    public override void _EnterTree()
    {
        _on_mouse_exited();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (hovering)
        {
            //Model.Rotate(Vector3.Up, (float) (0.5 * delta));
            Model.RotateY((float) (3 * delta));
            //Model.GlobalRotate(Vector3.Up, (float)(0.5 * delta));
            //Model.RotationDegrees = new Vector3(0, 30, 0);
            //GD.Print("rotate");
        }
    }

    private void _onFlowVisible()
    {
        _on_mouse_exited();
    }

    public void SetMesh(Node3D node)
    {
        Model.ReplaceBy(node);
        Model = node;
        var meshes = node.GetChildrenOfType<MeshInstance3D>();
        if (meshes.Count > 1)
        {
            GD.Print("this guy has " + meshes.Count);
        }
        foreach (var mesh in meshes)
        {
            mesh.LodBias = 128;
        }
        if(meshes.Count > 0)
        {
        var aabb = meshes[0].GetAabb();
        Camera3D.Size = Math.Max(aabb.Size.X, aabb.Size.Y) * 1.25f;
        }
        else
        {
            GD.Print("this guy has 0");
        }
    }

    public void SetMaterial(Material material)
    {
        if (material == null)
            return;
        var meshes = Model.GetChildrenOfType<MeshInstance3D>();
        foreach (var mesh in meshes)
        {
            mesh.MaterialOverride = material;
        }
    }

    public void _on_mouse_entered()
    {
        hovering = true;
        SubViewport.RenderTargetClearMode = SubViewport.ClearMode.Once;
        SubViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.WhenVisible;
    }

    public void _on_mouse_exited()
    {
        if (Model == null)
            return;
        hovering = false;
        Model.Rotation = new Vector3();
        this.SubViewport.RenderTargetClearMode = SubViewport.ClearMode.Once;
        this.SubViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
    }

}

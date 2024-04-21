using Godot;
using Godot.Sharp.Extras;
using System;

public partial class UiProperties : VBoxContainer
{
    public static UiProperties Instance {  get; private set; }

    [NodePath]
    public GridContainer GridProperties { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        Instance = this;
        GridProperties.RemoveAndQueueFreeChildren();

        //var az = new EditorInspector();
        //this.AddChild(az);
    }

    public void setTarget(object o)
    {

    }

}

using Godot;
using Godot.Sharp.Extras;
using System;

public partial class UiProperties : VBoxContainer
{
    [NodePath]
    public GridContainer GridProperties { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        GridProperties.RemoveAndQueueFreeChildren();
    }


}

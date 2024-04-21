using Godot;
using Godot.Sharp.Extras;
using System;
using Util.communication.events;

public partial class UiTreeExplorer : VBoxContainer
{

    [NodePath]
    public Tree TreeItems { get; set; }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        TreeItems.RemoveAndQueueFreeChildren();
    }


}

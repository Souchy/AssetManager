using Godot;
using Godot.Sharp.Extras;
using System;

public partial class UiList : PanelContainer
{
    [NodePath]
    public ItemList ItemList { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.OnReady();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void _on_btn_add_pressed()
    {

    }

    public void clear()
    {
        ItemList.RemoveAndQueueFreeChildren();
    }

}

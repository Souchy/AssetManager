using Godot;
using Godot.Sharp.Extras;
using System;

public partial class UiListItem : PanelContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.OnReady();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

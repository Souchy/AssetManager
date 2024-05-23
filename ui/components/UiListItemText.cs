using Godot;
using Godot.Sharp.Extras;
using System;

public partial class UiListItemText : UiListItem
{
    [NodePath]
    public TextEdit TextEdit { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

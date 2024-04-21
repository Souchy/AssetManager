using Godot;
using Godot.Sharp.Extras;
using System;
using System.IO;

public partial class ItemFolder : item
{
    public DirectoryInfo dir {  get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        this.OnReady();
    }

}

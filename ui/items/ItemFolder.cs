using AssetManager;
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
        base._Ready();
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if(@event is InputEventMouseButton me)
        {
            if(me.DoubleClick)
            {
                Explorer.Instance.select(dir.FullName);
            }
        }
    }

    public override void SetLabelName(string name)
    {
        this.LblName.Text = name;
    }

}

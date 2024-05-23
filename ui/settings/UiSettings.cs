using AssetManager;
using AssetManager.util;
using Godot;
using Godot.Sharp.Extras;
using System;

public partial class UiSettings : PanelContainer
{
    [NodePath]
    public UiList TagList { get; set; }
    [NodePath]
    public Tree Tree { get; set; }
    [Inject]
    public ConfigGeneral config {  get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.OnReady();
        this.Inject();

        var its = GD.Load<PackedScene>("res://ui/components/UiListItemText.tscn");
        var itemtext = its.Instantiate<UiListItemText>();
        TagList.ItemList.AddChild(itemtext);
        itemtext.TextEdit.Text = "Test on ready";
        foreach(var tag in config.Tags)
        {
            itemtext = its.Instantiate<UiListItemText>();
            TagList.ItemList.AddChild(itemtext);
            itemtext.TextEdit.Text = tag;
        }

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

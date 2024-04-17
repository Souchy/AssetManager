using Godot;
using Godot.Sharp.Extras;
using System;

public partial class item : PanelContainer
{
    [NodePath]
    public TextureRect Icon {  get; set; }
    [NodePath]
    public Label LblName { get; set; }

    public Action? Initializer { get; set; }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        this.OnReady();
	}

    public void SetName(string name)
    {
        this.LblName.Text = name.Replace("\\", "");
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void SetIcon()
    {
        
    }

    public void initialize() => Initializer?.Invoke();

}

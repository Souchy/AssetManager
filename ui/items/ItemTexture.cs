using Godot;
using Godot.Sharp.Extras;
using System;

public partial class ItemTexture : item
{
    [NodePath]
    public TextureRect TextureRect { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
	}

    public void SetTexture(Texture2D texture)
    {
        TextureRect.Texture = texture;
    }

    protected override void _on_click(InputEventMouseButton me)
    {
        base._on_click(me);
        if(me.ButtonIndex == MouseButton.Right)
        {
            // open a menu
            // from this menu, button to make a pack with all the selected items
        }
    }

}

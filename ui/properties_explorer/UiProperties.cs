using AssetManager;
using AssetManager.db;
using AssetManager.util;
using Godot;
using Godot.Sharp.Extras;
using System;
using System.Linq;
using Util.communication.events;

public partial class UiProperties : VBoxContainer
{
    public static UiProperties Instance { get; private set; }
    private item item { get; set; }

    [Inject]
    public ConfigGeneral config { get; set; }

    [NodePath]
    public GridContainer GridProperties { get; set; }
    //[NodePath]
    //public UiList TagList { get; set; }
    [NodePath]
    public HFlowContainer TagList { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        this.Inject();
        Instance = this;
        GridProperties.RemoveAndQueueFreeChildren();
        EventBus.centralBus.subscribe(this);
        //var az = new EditorInspector();
        //this.AddChild(az);

        foreach (var tag in config.Tags)
        {
            var tagBtn = new Button
            {
                Text = tag,
                ToggleMode = true
            };
            tagBtn.Pressed += () => TagBtn_Pressed(tagBtn);
            TagList.AddChild(tagBtn);
        }
    }

    private void TagBtn_Pressed(Button btn)
    {
        foreach (var item in item.SelectedItems)
        {
            var asset = item.getAsset();

            if (btn.ButtonPressed)
            {
                asset.Tags.Add(btn.Text);
            }
            else
            {
                asset.Tags.Remove(btn.Text);
            }
        }
        AssetFolder.CurrentFolder?.save();
    }

    public void setTarget(object o)
    {

    }

    [Subscribe("FlowSelected")]
    public void onSelectedItem(item item)
    {
        this.item = item;
        var asset = item.getAsset();

        // Clear properties
        GridProperties.RemoveAndQueueFreeChildren();
        //TagList.clear();

        // Fill properties
        //GridProperties.AddChild(new Label()
        //{
        //    Text = "Tags"
        //});
        //var flow = new HFlowContainer();
        //GridProperties.AddChild(flow);
        foreach (var btn in TagList.GetChildren<Button>())
        {
            var pressed = asset.Tags.Contains(btn.Text);
            btn.SetPressedNoSignal(pressed);
        }
    }

    public void onPropertyChanged()
    {
        //var asset = item.getAsset();
        //asset.Tags.Clear();
        //AssetFolder.CurrentFolder.save();
    }


}

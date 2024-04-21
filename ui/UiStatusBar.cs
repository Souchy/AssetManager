using AssetManager.loaders;
using Godot;
using Godot.Sharp.Extras;
using System;
using Util.communication.events;

public partial class UiStatusBar : PanelContainer
{

    public const string EventStatusHeader = "status.header";
    public const string EventStatusValue = "status.value";

    #region Nodes
    [NodePath]
    public Label LblStatusHeader { get; set; }
    [NodePath]
    public Label LblStatusValue { get; set; }
    [NodePath]
    public Label LblFpsValue { get; set; }
    #endregion

    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        LblStatusHeader.Text = "";
        LblStatusValue.Text = "";
    }

    public override void _Process(double delta)
    {
        if (LblFpsValue != null)
            LblFpsValue.Text = ((int) (1.0 / delta)).ToString();
    }

    [Subscribe(Loaders.EventLoadCount)]
    private void _updateLoadCount(int current, int total)
    {
        LblStatusHeader.Text = "Loaded:";
        LblStatusValue.Text = current + " / " + total;
    }

    [Subscribe(EventStatusHeader)]
    private void updateLblStatusHeader(string header = null)
        => LblStatusHeader.Text = header;
    [Subscribe(EventStatusValue)]
    private void updateLblStatusValue(string value = null)
        => LblStatusValue.Text = value;

}

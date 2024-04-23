using Godot;
using Godot.Sharp.Extras;
using System;
using Util.communication.events;

public partial class item : PanelContainer
{
    #region Nodes
    [NodePath] public TextureRect Icon { get; set; }
    [NodePath] public Label LblName { get; set; }
    [NodePath] public PanelContainer PanelHover { get; set; }
    [NodePath] public PanelContainer PanelSelected { get; set; }
    [NodePath] public Label LblExtension { get; set; }
    #endregion

    #region Properties
    public string Path {  get; set; }
    public bool IsHovered => PanelHover.Visible;
    public bool IsSelected => PanelSelected.Visible;
    public UiFlowView ParentFlow => this.GetAncestor<UiFlowView>();
    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnReady();
        EventBus.centralBus.subscribe(this);
        PanelHover.Visible = false;
        PanelSelected.Visible = false;

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        //if (this.IsHovered)
        //{
        //}
    }

    public virtual void SetLabelName(string name)
    {
        Path = name;
        var extIndex = name.LastIndexOf('.');
        if (extIndex >= 0)
        {
            this.LblName.Text = name[..extIndex].Replace("\\", "");
            this.LblExtension.Text = name[(extIndex+1)..];
        }
        else
        {
            this.LblName.Text = name.Replace("\\", "");
            this.LblExtension.Text = "";
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void SetIcon()
    {

    }

    public void Select(bool selected)
    {
        if (PanelSelected != null)
        {
            PanelSelected.Visible = selected;
        }
    }

    #region Signal handlers
    public void _on_gui_input(InputEvent e)
    {
        if (e is InputEventMouseButton me)
        {
            if (me.IsPressed())
            {
                _on_click(me);
            }
        }
    }
    protected virtual void _on_click(InputEventMouseButton me)
    {
        if(me.ButtonIndex == MouseButton.Left)
        {
            if (me.ShiftPressed)
            {
                ParentFlow.SelectShift(this);
            }
            else
            if (me.CtrlPressed)
            {
                ParentFlow.Select(this, !IsSelected);
            }
            else
            {
                var wasSelected = IsSelected;
                ParentFlow.UnselectItems();
                ParentFlow.Select(this, !wasSelected);
            }
        }
    }

    public virtual void _on_mouse_entered()
    {
        if (PanelHover != null)
            PanelHover.Visible = true;
    }

    public virtual void _on_mouse_exited()
    {
        if (PanelHover != null)
        {
            PanelHover.Visible = false;
            //LabelScroll.ScrollHorizontal = 0;
        }
    }
    #endregion
}

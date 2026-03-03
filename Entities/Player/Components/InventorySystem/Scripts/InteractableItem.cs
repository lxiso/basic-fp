using Godot;

public partial class InteractableItem : Node3D
{
    [Export] public MeshInstance3D itemHighlight;

    public void GainFocus()
    {
        itemHighlight.Visible = true;
    }

    public void LoseFocus()
    {
        itemHighlight.Visible = false;
    }
}
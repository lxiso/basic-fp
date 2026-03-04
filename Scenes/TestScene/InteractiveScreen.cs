using Godot;
using System;

public partial class InteractiveScreen : Node3D
{
    public Area3D area;
    public MeshInstance3D mesh;
    public SubViewport subViewport;

    public override void _Ready()
    {
        base._Ready();
        area = FindChild("Area3D") as Area3D;
        mesh = FindChild("MeshInstance3D") as MeshInstance3D;
        subViewport = FindChild("SubViewport") as SubViewport;

        area.InputEvent += OnInputEvent;
    }

    public void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
    {
        if (subViewport == null || mesh == null) return;
        Vector3 localPos = mesh.ToLocal(eventPosition);
        float u = localPos.X + 0.5f;
        float v = 0.5f - localPos.Y;
        Vector2 pixelPos = new Vector2(u * subViewport.Size.X, v * subViewport.Size.Y);
        InputEvent clonedEvent = (InputEvent)@event.Duplicate();

        if (clonedEvent is InputEventMouse mouseEvent)
        {
            mouseEvent.Position = pixelPos;
            mouseEvent.GlobalPosition = pixelPos;
        }
    }

}

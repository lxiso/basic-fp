using Godot;
using System;
using System.Collections.ObjectModel;

public partial class DamageArea : Area3D
{
    [Export] public float damage = 10;
    [Export(PropertyHint.Range, "0,32")] public uint maskLayer = 2;
    public override void _Ready()
    {
        base._Ready();
        CollisionMask = maskLayer;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var bodies = GetOverlappingBodies();
        
        if (bodies.Count > 0)
        {
            foreach(Node3D body in bodies)
            {
                if (body.IsInGroup("Player") || body is PlayerController)
                {
                    PlayerController player = body as PlayerController;
                    player.TakeDamage(damage);
                }
            }
        }
    }
}

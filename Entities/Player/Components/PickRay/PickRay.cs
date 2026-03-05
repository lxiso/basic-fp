using Godot;

public partial class PickRay : RayCast3D
{
    [Export] public float rayLength = 2.5f;
    [Export] public float springStiffness = 30f;
    [Export] public float springDamping = 10f;
    [Export] public float rotationSmoothness = 10f;
    
    private RigidBody3D _currentPicked;
    private Marker3D _holdPos;

    public override void _Ready()
    {
        base._Ready();
        TargetPosition = new Vector3(0, 0, -rayLength);
        _holdPos = new Marker3D();
        AddChild(_holdPos);
        _holdPos.Position = TargetPosition;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event.IsActionPressed("action_pick")) if (_currentPicked == null) PickObject(); else DropObject();
    }


    public void PickObject()
    {
        if (_currentPicked != null || !IsColliding()) return;
        if (IsColliding() && GetCollider() is RigidBody3D body)
        {
            if (!body.IsInGroup("Pickable")) return;

            _currentPicked = body;
            _currentPicked.GravityScale = 0f;
            _currentPicked.LinearVelocity = Vector3.Zero;
            _currentPicked.AngularVelocity = Vector3.Zero;
        }
    }

    public void DropObject()
    {
        if (_currentPicked == null) return;

        _currentPicked.GravityScale = 1;
        _currentPicked = null;
    }

    private void ApplyForces(double delta)
    {
        Vector3 currentPos = _currentPicked.GlobalPosition;
        Vector3 targetPos = _holdPos.GlobalPosition;
        Vector3 velocity = _currentPicked.LinearVelocity;

        Vector3 force = (targetPos - currentPos) * springStiffness - (velocity * springDamping);
        _currentPicked.ApplyCentralForce(force);

        Basis targetBasis = _holdPos.GlobalBasis;
        Quaternion targetRotation = targetBasis.GetRotationQuaternion();
        Quaternion currentRot = _currentPicked.GlobalBasis.GetRotationQuaternion();

        Quaternion nextRotation = currentRot.Slerp(targetRotation, (float)delta * rotationSmoothness);
        _currentPicked.GlobalBasis = new Basis(nextRotation);

        _currentPicked.AngularVelocity = Vector3.Zero;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (_currentPicked != null)
        {
            ApplyForces(delta);
        }
    }

}

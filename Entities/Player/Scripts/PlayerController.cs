using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public Camera3D camera;
    public Marker3D head;
    public CollisionShape3D collisionShape;

    [Export] public float walkSpeed = 5f;
    [Export] public float airMultiplier = 0.8f;
    [Export] public float runMultiplier = 1.5f;
    [Export] public float crouchMultiplier = 0.5f;
    [Export] public float jumpVelocity = 10f;
    [Export] public float crouchHeightMultiplier = 0.5f;
    [Export] public float mouseSens = .25f;
    [Export] public float inertia = .25f;
    public (float, float) mouseYLimits = (-90f, 90f);

    private float defaultHeight;
    private float crouchHeight;
    [Export]public bool isGravityEnabled = true;
    public bool isLerping = true;
    public Vector3 targetVelocity = Vector3.Zero;
    private Vector3 _currentVelocity = Vector3.Zero;
    public PState currentState;
    public Vector3 inputDir = Vector3.Zero;

    public void NodeSetup()
    {
        camera = FindChild("Camera3D") as Camera3D;
        head = FindChild("Head") as Marker3D;

        // Setting the collision shape and calculating default and crouch heights
        collisionShape = FindChild("CollisionShape3D") as CollisionShape3D;
        defaultHeight = (collisionShape.Shape as CapsuleShape3D).Height;
        crouchHeight = defaultHeight * crouchHeightMultiplier;
    }

    public override void _Ready()
    {
        base._Ready();
        NodeSetup();
        AdjustCameraHeight();
        ChangeState(new PGrounded());
    }

    public void ChangeState(PState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    public void AdjustCameraHeight()
    {
        if (collisionShape == null || camera == null) return;

        var shape = collisionShape.Shape as CapsuleShape3D;
        if (shape == null) return;

        float targetHeight = shape.Height - shape.Height / 4f;
        Vector3 headPosition = head.Position;
        headPosition.Y = targetHeight;
        head.Position = headPosition;
    }

    public void ApplyVelocity()
    {
        _currentVelocity.X = Mathf.Lerp(_currentVelocity.X, targetVelocity.X, isLerping ? inertia : 1f);
        _currentVelocity.Y = targetVelocity.Y;
        _currentVelocity.Z = Mathf.Lerp(_currentVelocity.Z, targetVelocity.Z, isLerping ? inertia : 1f);

        Velocity = _currentVelocity;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        currentState?.HandleInput(this, @event);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        currentState?.PhysicsUpdate(this, (float)delta);
        ApplyVelocity();
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        currentState?.Update(this, (float)delta);
    }

}

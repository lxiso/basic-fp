using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public Camera3D camera;
    public Marker3D head;
    public CollisionShape3D collisionShape;
    public ShapeCast3D crouchCheck;

    [Export] public float walkSpeed = 5f;
    [Export] public float airMultiplier = 0.8f;
    [Export] public float runMultiplier = 1.5f;
    [Export] public float crouchMultiplier = 0.5f;
    [Export] public float jumpVelocity = 10f;
    [Export] public float mouseSens = .25f;
    [Export] public float inertia = .25f;
    [Export] public float cameraInertia = .1f;
    [Export] public float maxFallSpeed = 190f;
    public (float, float) mouseYLimits = (-90f, 90f);

    private float _verticalVelocity = 0f;
    [Export] public float defaultHeight = 1.7f;
    [Export] public float crouchHeight = 0.85f;
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
        collisionShape = FindChild("CollisionShape3D") as CollisionShape3D;

        crouchCheck = FindChild("CrouchCheck") as ShapeCast3D;
        crouchCheck.TargetPosition = new Vector3(0, defaultHeight - 
        (collisionShape.Shape as CapsuleShape3D).Height / 4f, 0);
    }

    public override void _Ready()
    {
        base._Ready();
        NodeSetup();
        CalculateHeight(defaultHeight);
        ChangeState(new PGrounded());
    }

    public void ChangeState(PState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    public void MouseMotion(InputEventMouseMotion mouseMotion)
    {
        head.RotateX(-mouseMotion.Relative.Y * mouseSens * 0.01f);
        head.RotationDegrees = new Vector3(
            Mathf.Clamp(head.RotationDegrees.X, mouseYLimits.Item1, mouseYLimits.Item2),
            head.RotationDegrees.Y,
            head.RotationDegrees.Z
        );

        RotateY(-mouseMotion.Relative.X * mouseSens * 0.01f);
    }

    public void CalculateBobbing(double delta)
    {
        _verticalVelocity = Mathf.Lerp(_verticalVelocity, targetVelocity.Y, cameraInertia);
        _verticalVelocity = Mathf.Clamp(_verticalVelocity, -maxFallSpeed, float.MaxValue);
        camera.RotationDegrees = camera.RotationDegrees.Lerp(new Vector3(_verticalVelocity, 0, 
        -inputDir.X * 2.5f), cameraInertia);
    }

    public void CalculateHeight(float height)
    {
        if (collisionShape == null || camera == null) return;
        CapsuleShape3D shape = collisionShape.Shape as CapsuleShape3D;
        if (shape == null) return;

        if (shape.Height != height)
        {
            shape.Height = Mathf.Lerp(shape.Height, height, 0.1f);
        }

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
        CalculateBobbing(delta);
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        currentState?.Update(this, (float)delta);
    }

}

using System;
using Godot;

public partial class PlayerController : CharacterBody3D
{
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public Camera3D camera;
    public Marker3D head;
    public CollisionShape3D collisionShape;
    public ShapeCast3D crouchCheck;

    [Export] public float maxHealth = 100f;
    private float currentHealth;
    [Export] public double damageCooldown = .8f;
    private double currentDamageCooldown;
    [Export] public float walkSpeed = 5f;
    [Export] public float airMultiplier = 0.8f;
    [Export] public float runMultiplier = 1.5f;
    [Export] public float crouchMultiplier = 0.5f;
    [Export] public float jumpVelocity = 10f;
    [Export] public float mouseSens = .25f;
    [Export] public float inertia = .25f;
    [Export] public float cameraInertia = .25f;
    [Export] public float maxFallSpeed = 190f;
    [Export] public float maxTiltAngle = 25f;
    [Export] public float speedMaxTiltAngle = 10f;
    [Export] public float walkBob = 1f;
    [Export] public float runBob = 5f;
    [Export] public float crouchBob = 10f;
    [Export] public float walkBobFreq = 2f;
    [Export] public float runBobFreq = 6f;
    [Export] public float crouchBobFreq = 5f;
    [Export] public bool isLazyCameraOn = true;
    [Export] public float lazyCameraSmooth = 15f;
    private float _lastGlobalY;
    private float _verticalOffset = 0f;
    public float currentBobFreq;
    public float currentBob;
    private double _bobTimer = 0f;
    public (float, float) mouseYLimits = (-90f, 90f);

    private float _verticalVelocity = 0f;
    [Export] public float defaultHeight = 1.7f;
    [Export] public float crouchHeight = 0.85f;
    [Export] public bool isGravityEnabled = true;
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
        currentHealth = maxHealth;
        currentDamageCooldown = damageCooldown;
        NodeSetup();
        CalculateHeight(defaultHeight);
        _lastGlobalY = GlobalPosition.Y;
        ChangeState(new PGrounded());
    }

    public void ChangeState(PState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    public void LazyCamera(double delta)
    {
        if (isLazyCameraOn)
        {
            float currentGlobalY = GlobalPosition.Y;
            float deltaY = currentGlobalY - _lastGlobalY;

            _verticalOffset -= deltaY;
            _verticalOffset = Mathf.Lerp(_verticalOffset, 0f, (float)delta * lazyCameraSmooth);

            Vector3 newHeadPos = head.Position;
            newHeadPos.Y += _verticalOffset;
            head.Position = newHeadPos;
            _lastGlobalY = currentGlobalY;
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentDamageCooldown <= 0f)
        {
            currentDamageCooldown = damageCooldown;
            currentHealth -= amount;
            Math.Clamp(currentHealth, 0f, maxHealth);
        }
    }

    public void TakeHealing(float amount)
    {
        currentHealth += amount;
        Math.Clamp(currentHealth, 0f, maxHealth);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void MouseMotion(InputEventMouseMotion mouseMotion)
    {
        head.RotateX(-mouseMotion.Relative.Y * mouseSens * .01f);
        head.RotationDegrees = new Vector3(
          Mathf.Clamp(head.RotationDegrees.X, mouseYLimits.Item1, mouseYLimits.Item2),
          head.RotationDegrees.Y,
          head.RotationDegrees.Z
        );

        RotateY(-mouseMotion.Relative.X * mouseSens * .01f);
    }

    public void CalculateTilt(double delta)
    {
        _verticalVelocity = Mathf.Lerp(_verticalVelocity, targetVelocity.Y, cameraInertia);
        float targetRotX = 0f;

        if (!IsOnFloor())
        {
            targetRotX = _verticalVelocity / speedMaxTiltAngle * maxTiltAngle;
            targetRotX = Mathf.Clamp(targetRotX, -maxTiltAngle, maxTiltAngle);
        }

        Vector3 currentRot = camera.RotationDegrees;
        Vector3 targetRot = new Vector3(targetRotX, currentRot.Y, -inputDir.X * 2.5f);
        camera.RotationDegrees = currentRot.Lerp(targetRot, (float)delta * cameraInertia * 10f);
    }

    public void CalculateBob(double delta)
    {
        Vector2 horizontalVelocity = new Vector2(_currentVelocity.X, _currentVelocity.Z);
        float speed = horizontalVelocity.Length();

        if (speed > .1f && IsOnFloor())
        {
            _bobTimer += delta * speed * currentBobFreq;

            Vector2 bob = new Vector2((float)Mathf.Cos(_bobTimer * .5f) * currentBob,
            (float)Math.Sin(_bobTimer) * currentBob * .01f);

            Vector3 targetPos = new Vector3(bob.X, bob.Y, camera.Position.Z);
            camera.Position = camera.Position.Lerp(targetPos, (float)delta * cameraInertia * 10f);
        }
        else
        {
            _bobTimer = 0f;
            camera.Position = camera.Position.Lerp(Vector3.Zero, (float)delta * cameraInertia * 10f);
        }
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
        CalculateTilt(delta);
        CalculateBob(delta);
        LazyCamera(delta);
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        currentState?.Update(this, (float)delta);
        currentDamageCooldown -= delta;
    }

}
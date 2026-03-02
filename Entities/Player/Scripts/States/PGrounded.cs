using Godot;

public partial class PGrounded : PState
{
    private Vector2 _inputDirection = Vector2.Zero;
    private float _currentSpeed = 0f;

    public void EnterState(PlayerController player)
    {
        GD.Print($"{player.Name} entered {player.currentState.GetType().Name} State");
    }

    public void ExitState(PlayerController player)
    {
        GD.Print($"{player.Name} exited {player.currentState.GetType().Name} State");
    }

    public void HandleInput(PlayerController player, InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            player.head.RotateX(-mouseMotion.Relative.Y * player.mouseSens * 0.01f);
            player.head.RotationDegrees = new Vector3(
                Mathf.Clamp(player.head.RotationDegrees.X, -90, 90),
                player.head.RotationDegrees.Y,
                player.head.RotationDegrees.Z
            );

            player.RotateY(-mouseMotion.Relative.X * player.mouseSens * 0.01f);
        }

        if (@event.IsActionPressed("move_jump"))
        {
            player.targetVelocity.Y += player.jumpVelocity;
            player.ChangeState(new PAir());
        }
    }

    public void PhysicsUpdate(PlayerController player, float delta)
    {
        _inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back").Normalized();
        player.inputDir = new Vector3(_inputDirection.X, 0, _inputDirection.Y);
        _currentSpeed = Input.IsActionPressed("move_run") ? player.walkSpeed * player.runMultiplier : player.walkSpeed;
        player.targetVelocity.X = _inputDirection.X * _currentSpeed;
        player.targetVelocity.Z = _inputDirection.Y * _currentSpeed;
        player.targetVelocity = player.targetVelocity.Rotated(Vector3.Up, player.Rotation.Y);

        if (!player.IsOnFloor())
        {
            player.ChangeState(new PAir());
        }
    }

    public void Update(PlayerController player, float delta)
    {
        // Handle general updates specific to grounded state
    }
}
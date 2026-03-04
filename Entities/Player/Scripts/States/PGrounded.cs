using Godot;

public partial class PGrounded : PState
{
    private Vector2 _inputDirection = Vector2.Zero;
    private float _currentSpeed = 0f;

    public void EnterState(PlayerController player)
    {
        player.currentBob = player.walkBob;
        player.currentBobFreq = player.walkBobFreq;
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
            player.MouseMotion(mouseMotion);
        }

        if (@event.IsActionPressed("move_jump"))
        {
            player.targetVelocity.Y = player.jumpVelocity;
        }

        if (@event.IsActionPressed("move_crouch"))
        {
            player.ChangeState(new PCrouched());
        }
    }

    public void PhysicsUpdate(PlayerController player, float delta)
    {
        player.CalculateHeight(player.defaultHeight);

        _inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back").Normalized();
        player.inputDir.X = _inputDirection.X;
        player.inputDir.Z = _inputDirection.Y;
        _currentSpeed = Input.IsActionPressed("move_run") ? player.walkSpeed * player.runMultiplier : player.walkSpeed;
        if (Input.IsActionPressed("move_run")) player.currentBobFreq = player.runBobFreq; else player.currentBobFreq = player.walkBobFreq;
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
        
    }
}
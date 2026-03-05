using Godot;

public partial class PAir : PState
{
    private Vector2 _inputDirection = Vector2.Zero;
    private float _currentSpeed = 0f;

    public void EnterState(PlayerController player)
    {
        
    }

    public void ExitState(PlayerController player)
    {
        
    }

    public void HandleInput(PlayerController player, InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            player.MouseMotion(mouseMotion);
        }
    }

    public void PhysicsUpdate(PlayerController player, float delta)
    {
        player.CalculateHeight(player.defaultHeight);
        
        _inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back").Normalized();
        player.inputDir = new Vector3(_inputDirection.X, 0, _inputDirection.Y);
        _currentSpeed = player.walkSpeed * player.airMultiplier;
        player.targetVelocity.X = _inputDirection.X * _currentSpeed;
        player.targetVelocity.Y -= player.gravity * delta * (player.isGravityEnabled ? 1f : 0f);
        player.targetVelocity.Y = Mathf.Clamp(player.targetVelocity.Y, -player.maxFallSpeed, float.MaxValue);
        player.targetVelocity.Z = _inputDirection.Y * _currentSpeed;
        player.targetVelocity = player.targetVelocity.Rotated(Vector3.Up, player.Rotation.Y);
        
        if (player.IsOnFloor())
        {
            player.ChangeState(new PGrounded());
        }
    }

    public void Update(PlayerController player, float delta)
    {
        
    }
}
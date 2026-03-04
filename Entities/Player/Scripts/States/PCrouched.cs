using System;
using Godot;

public partial class PCrouched : PState
{
    private Vector2 _inputDirection = Vector2.Zero;
    private float _currentSpeed = 0f;
    private CapsuleShape3D _shape;

    public void EnterState(PlayerController player)
    {
        player.currentBob = player.crouchBob;
        player.currentBobFreq = player.crouchBobFreq;
        _shape = player.collisionShape.Shape as CapsuleShape3D;
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

        if (@event.IsActionPressed("move_crouch") && !player.crouchCheck.IsColliding())
        {
            player.ChangeState(new PGrounded());
        }
    }

    public void PhysicsUpdate(PlayerController player, float delta)
    {
        player.CalculateHeight(player.crouchHeight);

        _inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back").Normalized();
        player.inputDir.X = _inputDirection.X;
        player.inputDir.Z = _inputDirection.Y;
        _currentSpeed = player.walkSpeed * player.crouchMultiplier;
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
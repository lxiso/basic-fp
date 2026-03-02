using System.Diagnostics;
using Godot;

public partial class GameManager : Node
{
    public Node3D root;
    public PlayerController player;
    public Vector3 playerPosition;
    [Export] public float maxFallHeight = -10f;

    [Export] public bool debugMode = false;
    private Label currentState;
    private Label velocity;
    private Label position;

    public bool isPaused = false;

    public override void _Ready()
    {
        base._Ready();
        ProcessMode = ProcessModeEnum.Always;

        root = FindParent("Root") as Node3D;

        if (debugMode)
        {
            currentState = root.FindChild("CurrentState") as Label;
            velocity = root.FindChild("Velocity") as Label;
            position = root.FindChild("Position") as Label;
        }

        Input.MouseMode = Input.MouseModeEnum.Captured;

        player = root.FindChild("PlayerController") as PlayerController;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("ui_cancel"))
        {
            Pause();
        }
    }

    public void PlayerChecks()
    {
        if (player == null) return;
        
        playerPosition = player.GlobalPosition;

        if (playerPosition.Y < maxFallHeight)
        {
            player.GlobalPosition = new Vector3(0, 1, 0);
            player.Velocity = Vector3.Zero;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Debug();
        PlayerChecks();
    }

    public void Pause()
    {
        isPaused = !isPaused;
        GetTree().Paused = GetTree().Paused ? false : true;
    }

    public void Debug()
    {
        if (!debugMode || player == null) return;
        currentState.Text = $"State: {player.currentState.GetType().Name}";
        velocity.Text = $"Velocity:\n X:{player.Velocity.X:F2}\n Y:{player.Velocity.Y:F2}\n Z:{player.Velocity.Z:F2}";
        position.Text = $"Position: {player.GlobalPosition}";
    }

}

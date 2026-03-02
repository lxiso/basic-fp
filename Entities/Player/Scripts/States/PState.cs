using Godot;
public interface PState

{
    public void EnterState(PlayerController player);
    public void ExitState(PlayerController player);
    public void HandleInput(PlayerController player, InputEvent @event);
    public void PhysicsUpdate(PlayerController player, float delta);
    public void Update(PlayerController player, float delta);
}
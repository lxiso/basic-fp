using Godot;

public partial class MovingPlatform : AnimatableBody3D
{
    [Export] public Godot.Collections.Array<Marker3D> positions {get; set;} = new Godot.Collections.Array<Marker3D>();
    [Export] public double travelTime = 2f;
    [Export] public double waitTime = 1f;
    [Export] public Tween.TransitionType transitionType = Tween.TransitionType.Sine;
    [Export] public Tween.EaseType easeType = Tween.EaseType.InOut;

    private int _currentIndex;

    public override void _Ready()
    {
        base._Ready();

        if (positions.Count < 2)
        {
            GD.PushWarning($"{Name} needs 2 or more markers to move");
            return;
        }

        GlobalPosition = positions[0].GlobalPosition;
        MoveToNext();
    }

    private void MoveToNext()
    {
        _currentIndex = (_currentIndex + 1) % positions.Count;
        Vector3 nextTarget = positions[_currentIndex].GlobalPosition;
        Tween tween = CreateTween().SetTrans(transitionType).SetEase(easeType);
        tween.TweenProperty(this, "global_position", nextTarget, travelTime);
        tween.TweenInterval(waitTime);
        tween.Finished += MoveToNext;
    }

}

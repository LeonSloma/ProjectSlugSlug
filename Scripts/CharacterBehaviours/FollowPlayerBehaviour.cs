using Godot;

public partial class FollowPlayerBehaviour : CharacterBehaviour {
    [Export]
    public float Speed = 5;
    [Export]
    public float FollowDistance { get; private set; } = 2;

    public override bool CanRun { get => true; }

    private Character _character;
    private NavigationAgent3D _navigationAgent;
    private Vector2 _facingVector = Vector2.Up;
    private Vector3 _targetVelocity;

    public override void Init(StateMachine machine) {
        _character = machine.GetParent() as Character;
        _navigationAgent = _character.GetNode("NavigationAgent3D") as NavigationAgent3D;
    }

    public override void Run(double delta) {
        FollowPlayer();

        _targetVelocity.Y = _character.Velocity.Y;
        _character.Velocity = _character.Velocity.MoveToward(_targetVelocity, .25f);
    }

    private void FollowPlayer() {
        var playerDistance = 
            _character.GlobalPosition.DistanceTo(Player.Instance.GlobalPosition);
        if (playerDistance < FollowDistance) {
            _targetVelocity = Vector3.Zero;
            return;
        }
        _navigationAgent.TargetPosition = Player.Instance.GlobalPosition;
        Vector3 nextLocation = _navigationAgent.GetNextPathPosition();
        _targetVelocity = (nextLocation - _character.GlobalPosition).Normalized() * Speed;
    }

}

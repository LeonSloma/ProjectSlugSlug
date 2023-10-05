using Godot;

public partial class LeadPlayerBehaviour : CharacterBehaviour {
    [Export]
    public float Speed = 5;
    [Export]
    public float Tolerance = 1;
    [Export]
    public float WaitDistance = 5;
    [Export]
    public float PlayerGoalDistance = 2;

    public override bool CanRun => _lead;

    private Character _character;
    private NavigationAgent3D _navigationAgent;
    private Vector3 _targetPosition;
    private bool _lead = false;
    private bool _targetReached = false;
    private Vector3 _targetVelocity;
    private float _targetDistance;
    private float _playerDistance;

    public override void Init(StateMachine machine) {
        _character = machine.GetParent() as Character;
        _navigationAgent = _character.GetNode("NavigationAgent3D") as NavigationAgent3D;
    }

    public override void Run(double delta) {
        _targetDistance = _character.GlobalPosition.DistanceTo(_targetPosition);
        _playerDistance = 
            _character.GlobalPosition.DistanceTo(Player.Instance.GlobalPosition);

        if (!_targetReached) _targetVelocity = NavigateToTarget();
        else if (_playerDistance < Tolerance * 2) _lead = false;

        _targetVelocity.Y = _character.Velocity.Y;
        _character.Velocity = _character.Velocity.MoveToward(_targetVelocity, .25f);
    }

    private Vector3 NavigateToTarget() {
        if (_targetDistance < Tolerance) {
            _targetReached = true;
            return Vector3.Zero;
        }
        else if (_playerDistance > WaitDistance) return Vector3.Zero;
        
        if (_character.IsOnWall() && _character.IsOnFloor())
            _character.Velocity += Vector3.Up * 4.5f;
        
        _navigationAgent.TargetPosition = _targetPosition;
        Vector3 nextLocation = _navigationAgent.GetNextPathPosition();
        return (nextLocation - _character.GlobalPosition).Normalized() * Speed;
    }

    public void SetLocation(string characterName, string locationID) {
        if (_character.CharacterName == characterName) {
            _targetPosition = KeyLocation.Locations[locationID].GlobalPosition;
            _lead = true;
        }
    }
}
using Godot;

public partial class ShootTarget : CharacterBehaviour {
    public override bool CanRun => _character.Hostile && _delayTimer <= 0;

    public float Speed { get; private set; }

    private Character _character;
    private NavigationAgent3D _navigationAgent;
    private double _delayTimer;

    public override void Init(StateMachine machine) {
        _character = machine.GetParent() as Character;
        _navigationAgent = _character.GetNode("NavigationAgent3D") as NavigationAgent3D;
    }

    public override void Run(double delta) {
        var spaceState = _character.GetWorld3D().DirectSpaceState;
        var raycastQuery = PhysicsRayQueryParameters3D.Create(
            _character.FacePosition,
            Player.Instance.Position);
        var raycastResult = spaceState.IntersectRay(raycastQuery);
        if (raycastResult.Count == 0) return;

        var collider = raycastResult["collider"].As<CollisionObject3D>();
        if (collider is Player) {
            Projectile proj =
                _character.ActiveWeapon.Projectile.Instantiate() as Projectile;
            proj.Position = _character.FacePosition;
            proj.LookAtFromPosition(proj.Position, Player.Instance.FacePosition);
            proj.SetCollisionMaskValue(4,true);
            _character.GetParent().AddChild(proj);
            proj.Init();

            _delayTimer = _character.ActiveWeapon.FireDelay;
            return;
        }

        _navigationAgent.TargetPosition = Player.Instance.GlobalPosition;
        Vector3 nextLocation = _navigationAgent.GetNextPathPosition();
        Vector3 targetVelocity = (nextLocation - _character.GlobalPosition).Normalized() * Speed;
        targetVelocity.Y = _character.Velocity.Y;
        _character.Velocity = _character.Velocity.MoveToward(targetVelocity, .25f);
    }

    public override void _PhysicsProcess(double delta) {
        if (_delayTimer <= 0) return;
        _delayTimer -= delta;
    }
}

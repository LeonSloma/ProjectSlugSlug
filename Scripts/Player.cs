using Godot;

public partial class Player : CharacterBody3D {
	public static Player Instance {get; private set;}

	[Export]
	public float CameraSensitivity = 1;
	[Export]
	public float Speed = 5.0f;
	[Export]
	public const float JumpVelocity = 4.5f;
	[Export]
	public PackedScene FireProjectile;
	public Vector3 FacePosition => _camera.GlobalPosition;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private float _targetXRotation;
	private Vector2 _lookDirection;
	private Vector2 _moveDirection;

	private Camera3D _camera;
	private RayCast3D _raycast;
	private Sprite3D _brandon;

	public Player() : base() {
		Instance = this;
	}

	public override void _Ready() {
		EventSystem.Init();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera = GetNode("Camera") as Camera3D;
		_raycast = _camera.GetNode("RayCast") as RayCast3D;
		_brandon = GetNode("Brandon") as Sprite3D;
	}

	public override void _UnhandledInput(InputEvent e) {
		if (e is InputEventMouseMotion x) {
			_lookDirection = x.Relative * 0.001f;
			RotateCamera();
			return;
		}
		else if (e.IsActionPressed("fire")) Fire();
		else if (e.IsActionPressed("interact")) Interact(); 
	}

	public override void _PhysicsProcess(double delta) {
		Vector3 velocity = Velocity;
		
		if (!IsOnFloor())
			velocity.Y -= Gravity * (float)delta;

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		Vector2 inputDir = Input.GetVector(
			"move_left", "move_right",
			"move_forward", "move_backward"
			);
		
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero) {
			float velDelta = IsOnFloor() ? .25f * Speed : .025f * Speed;
			velocity.X = Mathf.MoveToward(velocity.X, direction.X * Speed, velDelta);
			velocity.Z = Mathf.MoveToward(Velocity.Z, direction.Z * Speed, velDelta);
		} else {
			float velDelta = IsOnFloor() ? Speed : .01f * Speed;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, velDelta);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, velDelta);
		}

		if (Rotation.X != _targetXRotation) {
			Vector3 targetRotation = new(_targetXRotation, Rotation.Y, Rotation.Z);
			Rotation.MoveToward(targetRotation, .25f);
		}

		Velocity = velocity;
		MoveAndSlide();
		
		if (_raycast.IsColliding()) {
			_brandon.Visible = true;
			_brandon.GlobalPosition = _raycast.GetCollisionPoint();
		}
		else _brandon.Visible = false;
	}

	public void SwitchGravity() {
		_targetXRotation = Rotation.X == 0 ? Mathf.Pi : 0;
	}

	public void SetCameraFocus(Vector3 focusPosition) {
		_camera.LookAt(focusPosition, Vector3.Up);
	}

	private void RotateCamera() {
		float xRotation = Mathf.Clamp(
			_camera.Rotation.X - _lookDirection.Y * CameraSensitivity,
			-1.5f, 1.5f
			);
		float yRotation = _camera.Rotation.Y -_lookDirection.X * CameraSensitivity;
		_camera.Rotation = new(xRotation, 0, 0);
		RotateY(yRotation);
	}
	
	private void Fire() {
		Projectile proj = FireProjectile.Instantiate() as Projectile;
		proj.SetCollisionMaskValue(2, true);
		GetParent().AddChild(proj);
		proj.GlobalPosition = new Vector3(
			_camera.GlobalPosition.X,
			_camera.GlobalPosition.Y - 0.2f,
			_camera.GlobalPosition.Z);
		if(_raycast.IsColliding())
			proj.LookAt(_raycast.GetCollisionPoint());
		else proj.GlobalRotation = _camera.GlobalRotation;
		proj.Init();
	}

	private void Interact() {
		if (_raycast.IsColliding()) {
			var col = _raycast.GetCollider();
			if (col is IInteractable i) i.Interact();
		}
	}

}

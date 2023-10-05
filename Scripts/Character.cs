using System;
using Godot;

public partial class Character : CharacterBody3D {
    const float DEG90 = Mathf.Pi / 2;
    [Export]
    public string CharacterName {get; private set;}
    [Export]
    public int Health {get; private set;}
    [Export]
    public bool Hostile {get; set;}
    [Export]
    public Weapon[] Weapons;
    public Weapon ActiveWeapon => Weapons[_activeWeaponIndex];
    public Vector3 FacePosition => _face.GlobalPosition;

    private uint _activeWeaponIndex = 0;
    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private Vector3 _gravityVector = Vector3.Up;
    private Node3D _face;
    private AnimationPlayer _animationPlayer;
    private Vector2 _facingVector;

    public override void _Ready() {
        ProcessPriority = 1;
        _animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        _face = GetNode("Face") as Node3D;
    }

    public override void _PhysicsProcess(double delta) {
        if (!IsOnFloor()) Velocity -= _gravity * _gravityVector * (float)delta;
        MoveAndSlide();
        // SelectFacingSprite();
    }
    
    public void TakeDamage(int damage) {
        Health -= damage;
        if (Health <= 0) Die();
    }

    private void SelectFacingSprite() {
        if (Velocity.X != 0 || Velocity.Z != 0)
            _facingVector = new(Velocity.X, Velocity.Z);

        LookAt(Player.Instance.GlobalPosition);

        float angleToPlayer = Rotation.Y - Vector2.Up.AngleTo(_facingVector);
        if (angleToPlayer > 0)
            if (angleToPlayer < DEG90) _animationPlayer.Play("back_right");
            else _animationPlayer.Play("front_left");
        else if (angleToPlayer < 0)
            if (angleToPlayer > -DEG90) _animationPlayer.Play("back_left");
            else _animationPlayer.Play("front_right");
    }

    public void GetCharacterPosition(string name) {
        if (CharacterName == name)
            DialogueSystem.DrawPlayerFocus(_face.GlobalPosition);
    }

    public void SetHostile(string isHostile) {
        GD.Print("hostile");
        Hostile = Boolean.Parse(isHostile);
    }

    private void Die() {
        QueueFree();
    }

}
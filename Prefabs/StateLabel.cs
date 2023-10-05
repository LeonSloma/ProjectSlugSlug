using Godot;
using System;

public partial class StateLabel : Label3D {
    private StateMachine _statemachine;

    public override void _Ready() {
        _statemachine = GetParent().GetNode("StateMachine") as StateMachine;
    }

    public override void _PhysicsProcess(double delta) {
        Text = _statemachine.State;
    }
}

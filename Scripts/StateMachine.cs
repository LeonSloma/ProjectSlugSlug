using Godot;

public partial class StateMachine : Node {
    public string State { get => _states[_currentState].Name; }

    private uint _currentState;

    private CharacterBehaviour[] _states;
    
    public override void _Ready() {
        // load states from SceneTree
        var children = GetChildren();
        _states = new CharacterBehaviour[children.Count];
        for (int i=0; i<children.Count; i++) {
            _states[i] = children[i] as CharacterBehaviour;
            _states[i].Init(this);
        }
    }

    public override void _PhysicsProcess(double delta) {
        for (uint i=0; i<_states.Length; i++)
            if(_states[i].CanRun) {
                _currentState = i;        
                break;
            }
        _states[_currentState].Run(delta);
    }

}
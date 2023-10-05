using Godot;

public abstract partial class CharacterBehaviour : Node {
    public abstract bool CanRun { get; }

    public abstract void Init(StateMachine machine);
    public abstract void Run(double delta);
}
using Godot;

public partial class EngageDialogueBehaviour : CharacterBehaviour {
    [Export]
    public float EngageDistance = 2;

    public override bool CanRun { get => CheckCanRun(); }

    private Character _character;

    public override void Init(StateMachine machine) {
        _character = machine.GetParent() as Character; 
    }

    public override void Run(double delta) {
        DialogueSystem.InitiateDialogue(
            DialogueSystem.UrgentDialogues[_character.CharacterName]);
        DialogueSystem.UrgentDialogues.Remove(_character.CharacterName);
    }

    private bool CheckCanRun() {
        return DialogueSystem.UrgentDialogues.ContainsKey(_character.CharacterName)
            && Player.Instance.GlobalPosition.DistanceTo(_character.GlobalPosition) < EngageDistance;
    }
}
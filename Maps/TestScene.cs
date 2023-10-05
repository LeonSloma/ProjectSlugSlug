using Godot;
using System;

public partial class TestScene : Node {
    public override void _EnterTree() {
        DialogueSystem.UrgentDialogues.Add("Slug", "Dialogue");
        GD.Print(DialogueSystem.UrgentDialogues.Count);
    }
}

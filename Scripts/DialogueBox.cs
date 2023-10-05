using Godot;
using System.Collections.Generic;

public partial class DialogueBox : CanvasLayer {
    private Node _vBox;
    public Label NameLabel;
    public Label TextLabel;
    
    public static List<OptionButton> OptionButtons = new();

    public override void _Ready() {
        _vBox = GetNode("MainPanel/VBox");
        NameLabel = _vBox.GetNode("NameLabel") as Label;
        TextLabel = _vBox.GetNode("TextPanel/TextLabel") as Label;
        DialogueSystem.RegisterDialogueBox(this);
    }

    public override void _UnhandledInput(InputEvent e) {
        if (e.IsActionPressed("ui_accept"))
            DialogueSystem.Next();
    }

    public void CreateOptionButton( DialogueOption o ) {
        OptionButton ob = new(o);
        OptionButtons.Add(ob);
        _vBox.AddChild(ob);
    }

    public partial class OptionButton : Button {
        private string _optionID;

        public OptionButton(DialogueOption o) : base() {
            _optionID = o.ID;
            Text = o.Text;
            Connect("button_down", new(this, "OnClick"));
        }

        public void OnClick() {
            GD.Print("hey");
            DialogueSystem.SelectOption(_optionID);
            OptionButtons.ForEach(o => o.QueueFree());            
            OptionButtons.Clear();
        }
    }
}

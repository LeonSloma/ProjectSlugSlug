using System;
using System.Text.Json;
using System.Collections.Generic;
using Godot;

public struct Dialogue {
    public DialogueLine  [] Lines   {get; set;}
    public DialogueOption[] Options {get; set;}
    public Event[] Events  {get; set;}
}

public struct DialogueLine {
    public string Character {get; set;}
    public string Text      {get; set;}
}

public struct DialogueOption {
    public string ID    {get; set;}
    public string Text  {get; set;}
}

public static class DialogueSystem {
    public static Dictionary<string, string> UrgentDialogues = new();
    public static Dictionary<string, string> InteractDialogues = new();

    private static DialogueBox _dialogueBox;
    private static Dictionary<string, Dialogue> _dialogues;
    private static Dialogue _currentDialogue;
    private static int _lineIndex;

    public static void InitiateDialogue(string dialogueId) {
        _dialogueBox.GetTree().Paused = true;
        _dialogueBox.Visible = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
        
        FileAccess file = FileAccess.Open(
            $"res://Assets/Dialogue/{dialogueId}.slug",
            FileAccess.ModeFlags.Read
            );
        String jsonString = file.GetAsText();
        file.Close();

        _dialogues = JsonSerializer.Deserialize<Dictionary<string, Dialogue>>(jsonString);
        AppModule.Initialize();

        _currentDialogue = _dialogues["Start"];
        _lineIndex = 0;

        _dialogueBox.NameLabel.Text = _currentDialogue.Lines[0].Character;
        _dialogueBox.TextLabel.Text = _currentDialogue.Lines[0].Text;
        _dialogueBox.GetTree().CallGroup("Characters", "GetCharacterPosition", _dialogueBox.NameLabel.Text);
    }

    public static void Next() {
        if (_currentDialogue.Lines.Length > ++_lineIndex) {
            _dialogueBox.NameLabel.Text = _currentDialogue.Lines[_lineIndex].Character;
            _dialogueBox.TextLabel.Text = _currentDialogue.Lines[_lineIndex].Text;
            _dialogueBox.GetTree().CallGroup("Characters", "GetCharacterPosition", _dialogueBox.NameLabel.Text);
        } else if (_currentDialogue.Options != null) {
            if (DialogueBox.OptionButtons.Count > 0) return;
            foreach (var option in _currentDialogue.Options)
                _dialogueBox.CreateOptionButton(option);
        } else {
            if (_currentDialogue.Events != null)
                foreach (Event e in _currentDialogue.Events) {
                    EventSystem.HandleEvent(e);
                }
            
            _dialogueBox.GetTree().Paused = false;
            _dialogueBox.Visible = false;
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public static void SelectOption(string optionID) {
        _currentDialogue = _dialogues[optionID];
        _lineIndex = 0;
        _dialogueBox.NameLabel.Text = _currentDialogue.Lines[0].Character;
        _dialogueBox.TextLabel.Text = _currentDialogue.Lines[0].Text;
    }

    public static void AddUrgentDialogue(string characterName, string dialogueId) {
        UrgentDialogues.Add(characterName, dialogueId);
    }

    public static void RegisterDialogueBox(DialogueBox db) {
        _dialogueBox = db;
    }

    public static void DrawPlayerFocus(Vector3 pos) {
        Player.Instance.SetCameraFocus(pos);
    }

}


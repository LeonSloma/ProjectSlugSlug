using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public static class EventSystem {
    public static Dictionary<string, Event> Events;    

    private static SceneTree _sceneTree;

    public static void Init() {
        _sceneTree = Player.Instance.GetTree();
        if(_sceneTree == null) GD.Print("uh oh");
    }

    public static void HandleEvent(Event e) {
        switch (e.Group) {
            case "DialogueSystem":
                GD.Print("dialogue");
                MethodInfo info = typeof(DialogueSystem).GetMethod(e.Method);
                info.Invoke(null, Array.ConvertAll(e.Args, o=>(object)o));
                break;
            default:
                GD.Print("group");
                _sceneTree.CallGroup(
                    e.Group,
                    e.Method,
                    Array.ConvertAll(e.Args, o=>(Variant)o)
                );
                break;
        }
    }
}

public struct Event {
    public string   Group  {get; set;}
    public string   Method {get; set;}
    public string[] Args   {get; set;}
}
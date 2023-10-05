using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class LoadArea : Node {


    public async void OnLoadEnter(Node3D body, string[] mapNames) {
        GD.Print("load");
        if (!(body is Player)) return;

        List<string> loadNames = new();
        foreach (var map in mapNames)
            if (World.Instance.GetNodeOrNull(map) == null)
                loadNames.Add(map);
        
        Node[] instances = new Node[loadNames.Count];
        
        await Task.Run(() =>{   
            for (int i = 0; i < loadNames.Count; i++)
                instances[i] =
                    GD.Load<PackedScene>("res://Maps/" + loadNames[i] + ".tscn").Instantiate();
        });
        foreach (var node in instances) World.Instance.AddChild(node);

    }

    public void OnUnloadEnter(Node3D body, string[] mapNames) {
        GD.Print("unload");
        if (!(body is Player)) return;

        Node mapNode;
        foreach (var map in mapNames) {
            GD.Print(map);
            mapNode = World.Instance.GetNodeOrNull(map);
            if (mapNode != null) mapNode.QueueFree();
        }
    }
}

using Godot;
using System.Collections.Generic;

public partial class KeyLocation : Node3D {
    public static Dictionary<string, KeyLocation> Locations = new();

    public override void _EnterTree() {
        Locations.Add(Name, this);
    }
}

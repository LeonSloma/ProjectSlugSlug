using Godot;
using System;

public partial class World : Node {
    public static World Instance { get; private set; }

    public override void _EnterTree() {
        Instance = this;
    }
}

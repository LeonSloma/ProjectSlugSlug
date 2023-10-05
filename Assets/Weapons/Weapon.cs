using Godot;

public partial class Weapon : Resource {
    [Export]
    public CompressedTexture2D sprite;
    [Export]
    public PackedScene Projectile;
    [Export]
    public double FireDelay;
}
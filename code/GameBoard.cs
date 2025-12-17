using Godot;

[GlobalClass]
public partial class GameBoard : RefCounted
{
	public Unit AddUnit(PlayerController pc, ModelType mt, Vector2I loc, Vector2I vel)
	{
		Unit unit = new Unit(pc, mt, loc, vel);
		return unit;
	}
}

using Godot;
using woelib.NegaMax;

[GlobalClass]
public partial class UnitAction : RefCounted, INMAction
{
	public string Name
	{
		get
		{
			return IsNoOp() ? "NoOp" : $"(#{UnitId} -> {Move})";
		}
	}

	public bool IsNoOp() { return UnitId < 0; }

	static UnitAction s_no_op = new UnitAction();
	internal static UnitAction NoOp => s_no_op;

	public int UnitId { get; private set; }
	public Vector2I Move { get; private set; }
	public UnitAction(Unit unit, Vector2I move)
	{
		UnitId = unit.Id;
		Move = move;
	}
	private UnitAction()
	{
		UnitId = -1;
		Move = Vector2I.Zero;
	}
}

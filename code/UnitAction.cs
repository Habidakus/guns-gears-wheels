using Godot;

[GlobalClass]
public partial class UnitAction : RefCounted
{
	public string Name
	{
		get
		{
			return IsNoOp() ? "NoOp" : $"({Unit.Name} -> {Move})";
		}
	}

	public bool IsNoOp() { return Unit == null; }

	static UnitAction s_no_op = new UnitAction();
	internal static UnitAction NoOp => s_no_op;

	public Unit Unit { get; private set; }
	public Vector2I Move { get; private set; }
	public UnitAction(Unit unit, Vector2I move)
	{
		Unit = unit;
		Move = move;
	}
	private UnitAction()
	{
		Unit = null;
		Move = Vector2I.Zero;
	}
}

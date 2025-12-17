using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class GameBoard : RefCounted
{
	private PlayerController _current_player;
	private List<Unit> _units = new List<Unit>();

	public Unit AddUnit(PlayerController pc, ModelType mt, Vector2I loc, Vector2I vel)
	{
		Unit unit = new Unit(pc, mt, loc, vel);
		_units.Add(unit);
		return unit;
	}

	public void ApplyAction(UnitAction action)
	{
		if (action.IsNoOp())
		{
			return;
		}

		action.Unit.Update(action);
	}

	public GameBoard GetChild(UnitAction action)
	{
		return new GameBoard(this, action);
	}

	public GameBoard()
	{
	}

	private GameBoard(GameBoard parent, UnitAction action)
	{
		_current_player = parent._current_player.Opponent;
		_units = new();
		foreach (var unit in parent._units)
		{
			_units.Add(unit.Clone());
		}

		ApplyAction(action);
	}

	private int CompareTurnOrder(Unit a, Unit b)
	{
		if (a.NextTurn != b.NextTurn)
		{
			return a.NextTurn.CompareTo(b.NextTurn);
		}

		if (a.Owner != b.Owner)
		{
			if (a.Owner == _current_player)
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}

		return a.TurnBreaker.CompareTo(b.TurnBreaker);
	}

	internal Unit GetCurrentUnit()
	{
		_units.Sort(CompareTurnOrder);
		return _units.First();
	}

	internal IEnumerable<UnitAction> GetValidMoves(Unit unit)
	{
		foreach (Vector2I move in unit.Model.GetMoves(unit.Velocity))
		{
			yield return new UnitAction(unit, move);
		}
	}
}

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using woelib.NegaMax;

public class BoardScore : NMScore
{
	PlayerController _player { get; }
	int _weaponsReady = 0;

	private BoardScore(PlayerController player, int weaponsReady)
	{
		_player = player;
		_weaponsReady = weaponsReady;
	}

	public BoardScore(PlayerController score_player, List<Unit> units, TileMapLayer mapLayer)
	{
		_player = score_player;
		foreach (Unit attacker in units)
		{
			if (!attacker.IsAlive)
				continue;
			foreach (Weapon weapon in attacker.Weapons)
			{
				bool hasValidTarget = false;
				for (int i = 0; hasValidTarget == false && i < units.Count; ++i)
				{
					Unit target = units[i];
					if (target == attacker)
						continue;
					if (!target.IsAlive)
						continue;
					if (attacker.Owner == target.Owner)
						continue;

					hasValidTarget |= weapon.CanHit(mapLayer, attacker, target);
				}

				if (hasValidTarget)
				{
					if (attacker.Owner == _player)
						_weaponsReady += 1;
					else
						_weaponsReady -= 1;
				}
			}
		}
	}

	public override string ToString()
	{
		return $"Score for {_player.Name}: {_weaponsReady}";
	}

	protected override NMScore CreateReverse()
	{
		return new BoardScore(_player, 0 -  _weaponsReady);
	}

	protected override bool IsGreaterThan(NMScore other)
	{
		if (other is BoardScore otherScore)
		{
			return _weaponsReady > otherScore._weaponsReady;
		}
		else
		{
			throw new Exception($"Can't compare BoardScore({this}) with {other}");
		}
	}
}

[GlobalClass]
public partial class GameBoard : RefCounted, woelib.NegaMax.INMGameState
{
	private PlayerController _current_player;
	private PlayerController _score_player;
	private TileMapLayer _map_layer;
	private List<Unit> _units = new List<Unit>();
	internal PlayerController CurrentPlayer { get { return _current_player; } }

	public Unit AddUnit(PlayerController pc, ModelType mt, Vector2I loc, Vector2I vel)
	{
		Unit unit = new Unit(pc, _map_layer, mt, loc, vel);
		_units.Add(unit);
		return unit;
	}

	public void ApplyAction(UnitAction action)
	{
		if (action.IsNoOp())
		{
			return;
		}

		GetUnitById(action.UnitId).Update(_map_layer, action);
	}

	public Unit GetUnitById(int id)
	{
		foreach (Unit unit in _units)
		{
			if (unit.Id == id)
				return unit;
		}

		throw new Exception($"Unit #{id} not found in game board");
	}

	public GameBoard GetChild(UnitAction action)
	{
		return new GameBoard(this, action);
	}

	public GameBoard() { }
	public void SetMap(TileMapLayer mapLayer)
	{
		_map_layer = mapLayer;
	}

	public void SetCurrentPlayer(PlayerController pc)
	{
		_current_player = pc;
		_score_player = pc;
	}

	private GameBoard(GameBoard parent, UnitAction action)
	{
		_current_player = parent._current_player.Opponent;
		_score_player = parent._score_player;
		_map_layer = parent._map_layer;
		_units = new();
		foreach (Unit unit in parent._units)
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

	/// <summary>
	/// Returns true if there are any moves that the current player could perform in the current game state.
	/// </summary>
	public bool HasMoves => GetCurrentUnit().HasMoves;

	/// <summary>
	/// Returns the list of all legal moves that the current player could make in the current game state.
	/// </summary>
	public IEnumerable<INMAction> SortedMoves
	{
		get
		{
			Unit current_unit = GetCurrentUnit();
			// #TODO: Sort these moves
			return GetValidMoves(current_unit);
		}
	}

	NMScore INMGameState.Score { get { return new BoardScore(_score_player, _units, _map_layer); } }

	/// <summary>
	/// Returns a new game state object that represents what will happen, deterministically, to the current game
	/// state object if the given <see cref="INMAction"/> is applied to it.
	/// </summary>
	public INMGameState CreateChild(INMAction action)
	{
		if (action is UnitAction ua)
			return GetChild(ua);
		else
			throw new ArgumentException("action is not of type UnitAction");
	}
}

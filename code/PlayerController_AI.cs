using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class PlayerController_AI : PlayerController
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	UnitAction _chosen_action = null;
	Callable? _callback = null;

	public PlayerController_AI()
	{
		Name = "AI Player #" + ID;
	}

	public override void RequestMove(GameBoard gameBoard, Callable callback)
	{
		_callback = callback;
		Unit unit = gameBoard.GetCurrentUnit();
		if (unit.Owner != this)
		{
			_chosen_action = UnitAction.NoOp;
			return;
		}

		IEnumerable<UnitAction> actions = gameBoard.GetValidMoves(unit);
		if (!actions.Any())
		{
			throw new Exception($"{Name} has no valid moves for {unit.Name}");
		}
		else
		{
			List<UnitAction> actionList = actions.ToList();
			_chosen_action = actionList[rng.RandiRange(0, actionList.Count - 1)];
		}
	}

	public override void ProcessMove(float timeoutSpan)
	{
		if (_callback != null)
		{
			if (_chosen_action != null)
			{
				GD.Print($"{Name} submitting action {_chosen_action.Name}");
				_callback?.Call(_chosen_action);
				_chosen_action = null;
				_callback = null;
			}
		}
	}
}

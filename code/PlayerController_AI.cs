using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class PlayerController_AI : PlayerController
{
	//RandomNumberGenerator rng = new RandomNumberGenerator();
	Callable? _callback = null;
	woelib.NegaMax.Calculator _calculator = new();
	woelib.NegaMax.Request _request = null;

	public PlayerController_AI()
	{
		Name = "AI Player #" + ID;
		switch(ID % 2)
		{
			case 0:
				MoveColor = Colors.DarkBlue;
				GunColor = Colors.LightBlue;
				break;
			case 1:
				MoveColor = Colors.DarkGreen;
				GunColor = Colors.LightGreen;
				break;
		}
	}

	public override void RequestMove(GameBoard gameBoard, Callable callback)
	{
		_callback = callback;
		Unit unit = gameBoard.GetCurrentUnit();
		if (unit.Owner == null)
			throw new Exception("Current owner of unit is null");
		if (gameBoard.CurrentPlayer == null)
			throw new Exception("Current player is null");
		if (unit.Owner != this)
		{
			_callback?.Call(UnitAction.NoOp);
			_callback = null;
			return;
		}

		_request = new(gameBoard, depth: 7, TimeSpan.FromSeconds(0.05));
	}

	public override void ProcessMove(float timeoutSpan)
	{
		if (_callback != null)
		{
			woelib.NegaMax.IResponse response = woelib.NegaMax.Calculator.GetBestAction(_request);
			if (response is woelib.NegaMax.PausedResponse pausedResponse)
			{
				//GD.Print("Got pause Response");
				_request = pausedResponse.ContinuationRequest;
			}
			else if (response is woelib.NegaMax.ResolvedResponse resolvedResponse)
			{
				UnitAction chosen_action = resolvedResponse.Action as UnitAction;
				if (chosen_action != null)
				{
					GD.Print($"{Name} submitting action {chosen_action.Name} with score: {resolvedResponse.Score}");
					_callback?.Call(chosen_action);
					_callback = null;
					_request = null;
				}
				else
				{
					_callback = null;
					Unit unit = (_request.GameState as GameBoard).GetCurrentUnit();
					_request = null;
					throw new Exception($"{Name} could not determine a valid action for {unit.Name}.");
				}
			}
		}
	}
}

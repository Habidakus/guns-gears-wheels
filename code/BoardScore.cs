using Godot;
using System;
using System.Collections.Generic;
using woelib.NegaMax;

public class BoardScore : NMScore
{
	PlayerController _player { get; }
	int _weaponsReady = 0;
	float _turnNeeded = 0;

	private BoardScore(PlayerController player, int weaponsReady, float turnNeeded)
	{
		_player = player;
		_weaponsReady = weaponsReady;
		_turnNeeded = turnNeeded;
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
				float minTurnNeeded = float.MaxValue;
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

					if (weapon.CanHit(mapLayer, attacker, target, out float turnNeeded))
					{
						hasValidTarget = true;
					}
					else
					{
						minTurnNeeded = Mathf.Min(minTurnNeeded, turnNeeded);
					}
				}

				if (attacker.Owner == _player)
				{
					if (hasValidTarget)
					{
						_weaponsReady += 1;
					}
					else
					{
						_turnNeeded += minTurnNeeded;
					}
				}
				else
				{
					//if (hasValidTarget)
					//{
					//	_weaponsReady -= 1;
					//}
					//else
					//{
					//	_turnNeeded -= minTurnNeeded;
					//}
				}
			}
		}
	}

	public override string ToString()
	{
		return $"Score for {_player.Name}: {_weaponsReady}, {_turnNeeded}";
	}

	protected override NMScore CreateReverse()
	{
		return new BoardScore(_player, 0 -  _weaponsReady, 0 - _turnNeeded);
	}

	protected override bool IsGreaterThan(NMScore other)
	{
		if (other is BoardScore otherScore)
		{
			if (_weaponsReady != otherScore._weaponsReady)
			{
				return _weaponsReady > otherScore._weaponsReady;
			}
			else
			{
				return _turnNeeded < otherScore._turnNeeded;
			}
		}
		else
		{
			throw new Exception($"Can't compare BoardScore({this}) with {other}");
		}
	}
}

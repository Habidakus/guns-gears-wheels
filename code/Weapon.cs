using Godot;
using System;

public class Weapon
{
	public float m_rotation;
	public float m_totalArc;
	public int m_range;

	public static Weapon ForwardMG => s_forwardMG;
	public static Weapon LeftSidePistol => s_leftSidePistol;
	public static Weapon ForwardPistol => s_forwardPistol;

	public bool CanHit(TileMapLayer mapLayer, Unit attacker, Unit defender, out float turnNeeded)
	{
		Vector2 attacker_loc = mapLayer.MapToLocal(attacker.Location);
		float rot = attacker.Rotation + m_rotation;
		turnNeeded = float.MaxValue;
		foreach (Vector2I hex in defender.AllHexes)
		{
			if (CanHit(attacker_loc, mapLayer.MapToLocal(hex), rot, out float hexTurnNeeded))
			{
				turnNeeded = 0;
				return true;
			}
			else
			{
				turnNeeded = Mathf.Min(turnNeeded, hexTurnNeeded);
			}
		}

		return false;
	}

	public bool CanHit(Vector2 start, Vector2 end, float rot, out float turnNeeded)
	{
		//if (start.DistanceSquaredTo(end) > (m_range * m_range))
		//	return false;
		float angle = start.AngleTo(end);
		float away = Mathf.Abs(Mathf.AngleDifference(angle, rot));
		if (away <= (m_totalArc / 2f))
		{
			turnNeeded = 0;
			return true;
		}
		else
		{
			turnNeeded = away - (m_totalArc / 2f);
			return false;
		}
	}

	private static Weapon s_forwardMG = new Weapon(0, MathF.Tau / 8f, 12);
	private static Weapon s_forwardPistol = new Weapon(0, MathF.Tau / 4f, 8);
	private static Weapon s_leftSidePistol = new Weapon(0 - MathF.PI / 2f, MathF.Tau / 4f, 8);

	private Weapon(float rotation, float arc, int range)
	{
		m_rotation = rotation;
		m_totalArc = arc;
		m_range = range;
	}
}

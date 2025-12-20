using Godot;
using System;

public static class HexUtils
{
	/// <summary>
	/// Calculates the "hex distance" (pathfinding distance) between two hexes using axial coordinates.
	/// </summary>
	public static int Distance(Vector2I a, Vector2I b)
	{
		// Calculate the difference in coordinates
		int deltaX = a.X - b.X;
		int deltaY = a.Y - b.Y;

		// The third cubic coordinate can be derived as S = - X - Y
		int deltaS = - deltaX - deltaY;

		// The distance is the Manhattan distance in the cubic system divided by 2
		return (Math.Abs(deltaX) + Math.Abs(deltaY) + Math.Abs(deltaS)) / 2;

		// Alternative, equivalent formula for axial coordinates:
		// return (Math.Abs(deltaX) + Math.Abs(deltaX + deltaY) + Math.Abs(deltaY)) / 2;

		// Another equivalent formula, using the max of the absolute differences:
		// return Math.Max(Math.Abs(deltaX), Math.Max(Math.Abs(deltaY), Math.Abs(deltaS)));
	}
}

public class Weapon
{
	public float m_rotation;
	public float m_totalArc;
	public int m_range;

	public static Weapon ForwardMG => s_forwardMG;
	public static Weapon LeftSidePistol => s_leftSidePistol;
	public static Weapon ForwardPistol => s_forwardPistol;

	public bool CanHit(TileMapLayer mapLayer, Unit attacker, Unit defender, out float turnNeeded, out Vector2I hexHit)
	{
		Vector2 global_gun_pos = mapLayer.ToGlobal(mapLayer.MapToLocal(attacker.Location));
		float rot = attacker.Rotation + m_rotation;
		turnNeeded = float.MaxValue;
		foreach (Vector2I hex in defender.AllHexes)
		{
			if (CanHitAngle(global_gun_pos, mapLayer.ToGlobal(mapLayer.MapToLocal(hex)), rot, out float hexTurnNeeded))
			{
				if (HexUtils.Distance(attacker.Location, hex) > m_range)
				{
					turnNeeded = Mathf.Min(turnNeeded, hexTurnNeeded);
				}
				else
				{
					turnNeeded = 0;
					hexHit = hex;
					return true;
				}
			}
			else
			{
				turnNeeded = Mathf.Min(turnNeeded, hexTurnNeeded);
			}
		}

		hexHit = Vector2I.Zero;
		return false;
	}

	public bool CanHit(TileMapLayer mapLayer, Unit attacker, Vector2I hex)
	{
		if (HexUtils.Distance(attacker.Location, hex) > m_range)
		{
			return false;
		}

		Vector2 global_gun_pos = mapLayer.ToGlobal(mapLayer.MapToLocal(attacker.Location));
		float rot = attacker.Rotation + m_rotation;
		return CanHitAngle(global_gun_pos, mapLayer.ToGlobal(mapLayer.MapToLocal(hex)), rot, out float _);
	}

	public bool CanHitAngle(Vector2 start, Vector2 end, float angleGunIsAimedAt, out float turnNeeded)
	{
		float angleGunToTarget = (end - start).Angle();
		float diff = Mathf.Abs(Mathf.Wrap(angleGunToTarget - angleGunIsAimedAt, -Mathf.Pi, Mathf.Pi));
		if (diff <= (m_totalArc / 2f))
		{
			turnNeeded = 0;
			return true;
		}
		else
		{
			turnNeeded = diff - (m_totalArc / 2f);
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

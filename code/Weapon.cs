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

	public bool CanHit(TileMapLayer mapLayer, Unit attacker, Unit defender)
	{
		Vector2 attacker_loc = mapLayer.MapToLocal(attacker.Location);
		float rot = attacker.Rotation + m_rotation;
		foreach (Vector2I hex in defender.AllHexes)
		{
			if (CanHit(attacker_loc, mapLayer.MapToLocal(hex), rot))
				return true;
		}

		return false;
	}

	public bool CanHit(Vector2 start, Vector2 end, float rot)
	{
		if (start.DistanceSquaredTo(end) > (m_range * m_range))
			return false;
		float angle = start.AngleTo(end);
		return Mathf.AngleDifference(angle, rot) < (m_totalArc / 2f);
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

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
		Vector2 global_gun_pos = mapLayer.ToGlobal(mapLayer.MapToLocal(attacker.Location));
		float rot = attacker.Rotation + m_rotation;
		turnNeeded = float.MaxValue;
		foreach (Vector2I hex in defender.AllHexes)
		{
			if (CanHit(global_gun_pos, mapLayer.ToGlobal(mapLayer.MapToLocal(hex)), rot, out float hexTurnNeeded))
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

	public bool CanHit(TileMapLayer mapLayer, Unit attacker, Vector2I hex)
	{
		Vector2 global_gun_pos = mapLayer.ToGlobal(mapLayer.MapToLocal(attacker.Location));
		float rot = attacker.Rotation + m_rotation;
		return CanHit(global_gun_pos, mapLayer.ToGlobal(mapLayer.MapToLocal(hex)), rot, out float _);
	}

	public bool CanHit(Vector2 start, Vector2 end, float angleGunIsAimedAt, out float turnNeeded)
	{
		float angleGunToTarget = (end - start).Angle();
		float diff = Mathf.Abs(Mathf.Wrap(angleGunToTarget - angleGunIsAimedAt, -Mathf.Pi, Mathf.Pi));
		// float away = Mathf.Abs(Mathf.AngleDifference(angle, rot));
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

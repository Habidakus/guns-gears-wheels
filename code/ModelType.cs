using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class ModelType : GodotObject
{
	static ModelType s_motorcycle = new ModelType("Motorcycle", 5, [
		new Vector2I(0,0),
		new Vector2I(-1, 0),
	]);
	public static ModelType GetMotorcycle()
	{
		return s_motorcycle;
	}

	static ModelType s_car = new ModelType("Car", 7, [
		new Vector2I(1, -1),
		new Vector2I(0,-1),
		new Vector2I(-1, -1),
		new Vector2I(-2, -1),
		//new Vector2I(-3, -1),
		new Vector2I(0,0),
		new Vector2I(-1, 0),
		new Vector2I(-2, 0),
		new Vector2I(-3, 0),
		new Vector2I(0,1),
		new Vector2I(-1, 1),
		new Vector2I(-2, 1),
		new Vector2I(-3, 1),
		//new Vector2I(-4, 1),
		]);
	public static ModelType GetCar()
	{
		return s_car;
	}

	static ModelType s_sidecar = new ModelType("Sidecar", 6, [
		new Vector2I(0,0),
		new Vector2I(-1, 0),
		new Vector2I(-1, 1),
	]);
	public static ModelType GetSidecar()
	{
		return s_sidecar;
	}

	public static ModelType Van;
	public static ModelType EighteenWheeler;

	public Vector2I[] OccupiedOffsets { get; private set; } = [new Vector2I(0, 0)];
	public int TurnWait { get; }
	public string Name { get; }

	private ModelType(string name, int turnWait, Vector2I[] occupiedOffsets)
	{
		Name = name;
		TurnWait = turnWait;
		OccupiedOffsets = occupiedOffsets;
	}

	public IEnumerable<Vector2I> GetGlobalOccupiedHexes(Vector2I origin, float rotation)
	{
		// rotation = rotation - Mathf.Tau * Mathf.Floor((rotation + Mathf.Pi) / Mathf.Tau);
		rotation += Mathf.Pi / 6;
		int face = (int)Mathf.Floor(rotation / (Mathf.Pi / 3));
		face = (face % 6 + 6) % 6;

		switch (face)
		{
			case 0:
				foreach (Vector2I offset in OccupiedOffsets)
				{
					yield return origin + offset;
				}
				break;
			case 1:
				foreach (Vector2I offset in OccupiedOffsets)
				{
					yield return origin + new Vector2I(-offset.Y, offset.X + offset.Y);
				}
				break;
			case 2:
				foreach (Vector2I offset in OccupiedOffsets)
				{
					yield return origin + new Vector2I(-offset.Y - offset.X, offset.X);
				}
				break;
			case 3:
				foreach (Vector2I offset in OccupiedOffsets)
				{
					yield return origin - offset;
				}
				break;
			case 4:
				foreach (Vector2I offset in OccupiedOffsets)
				{
					yield return origin + new Vector2I(offset.Y, - offset.X - offset.Y);
				}
				break;
			case 5:
				foreach (Vector2I offset in OccupiedOffsets)
				{
					yield return origin + new Vector2I(offset.Y + offset.X, -offset.X);
				}
				break;
		}
	}

	internal IEnumerable<Vector2I> GetMoves(Vector2I velocity)
	{
		yield return velocity;
		yield return velocity + new Vector2I(1, 0);  // 0
		yield return velocity + new Vector2I(0, 1);  // 1
		yield return velocity + new Vector2I(-1, 1); // 2
		yield return velocity + new Vector2I(-1, 0); // 3
		yield return velocity + new Vector2I(0, -1); // 4
		yield return velocity + new Vector2I(1, -1); // 5
	}
}

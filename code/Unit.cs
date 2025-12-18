using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Unit : RefCounted
{
	static int s_nextId = 0;

	public int Id { get; private set; }
	public PlayerController Owner { get; private set; }
	public ModelType Model { get; private set; }
	public Vector2I Location { get; set; }
	public Vector2I Velocity { get; set; }
	public float Rotation { get; private set; }

	public bool IsAlive => true;
	public bool HasMoves => IsAlive;
	public int TurnBreaker => Id;
	public int NextTurn { get; private set; } = 0;

	public string Name
	{
		get
		{
			return $"{Model.Name} #{Id}";
		}
	}

	public IEnumerable<Weapon> Weapons => Model.Weapons;

	public IEnumerable<Vector2I> AllHexes
	{
		get
		{
			return Model.GetGlobalOccupiedHexes(Location, Rotation);
		}
	}

	internal Unit(PlayerController owner, TileMapLayer mapLayer, ModelType model, Vector2I loc, Vector2I velocity)
	{
		Id = ++s_nextId;
		Owner = owner;
		Model = model;
		Location = loc;
		Velocity = velocity;
		NextTurn = model.TurnWait;
		Rotation = CalculateRotation(mapLayer);
	}

	private Unit(Unit cloneParent)
	{
		Id = cloneParent.Id;
		Owner = cloneParent.Owner;
		Model = cloneParent.Model;
		Location = new Vector2I(cloneParent.Location.X, cloneParent.Location.Y);
		Velocity = new Vector2I(cloneParent.Velocity.X, cloneParent.Velocity.Y);
		NextTurn = cloneParent.NextTurn;
		Rotation = cloneParent.Rotation;
	}

	public void Update(TileMapLayer mapLayer, UnitAction action)
	{
		Location = Location + action.Move;
		Velocity = action.Move;
		NextTurn += Model.TurnWait;
		Rotation = CalculateRotation(mapLayer);
	}

	private float CalculateRotation(TileMapLayer mapLayer)
	{
		return mapLayer.MapToLocal(Vector2I.Zero).AngleTo(mapLayer.MapToLocal(Velocity));
	}

	internal Unit Clone()
	{
		return new Unit(this);
	}
}

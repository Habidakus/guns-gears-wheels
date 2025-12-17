using Godot;
using System;

[GlobalClass]
public partial class Unit : RefCounted
{
	static int s_nextId = 0;
	public int Id { get; private set; }
	public PlayerController Owner { get; private set; }
	public ModelType Model { get; private set; }
	public Vector2I Location { get; set; }
	public Vector2I Velocity { get; set; }

	public bool IsAlive => true;
	public int TurnBreaker => Id;
	public int NextTurn { get; private set; } = 0;

	public string Name
	{
		get
		{
			return $"{Model.Name} #{Id}";
		}
	}

	internal Unit(PlayerController owner, ModelType model, Vector2I loc, Vector2I velocity)
	{
		Id = ++s_nextId;
		Owner = owner;
		Model = model;
		Location = loc;
		Velocity = velocity;
		NextTurn = model.TurnWait;
	}

	private Unit(Unit cloneParent)
	{
		Id = cloneParent.Id;
		Owner = cloneParent.Owner;
		Model = cloneParent.Model;
		Location = cloneParent.Location;
		Velocity = cloneParent.Velocity;
		NextTurn = cloneParent.NextTurn;
	}

	public void Update(UnitAction action)
	{
		Location += action.Move;
		Velocity = action.Move;
		NextTurn += Model.TurnWait;
	}

	internal Unit Clone()
	{
		return new Unit(this);
	}
}

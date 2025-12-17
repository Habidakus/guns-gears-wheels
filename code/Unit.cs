using Godot;

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

	internal Unit(PlayerController owner, ModelType model, Vector2I loc, Vector2I velocity)
	{
		Id = ++s_nextId;
		Owner = owner;
		Model = model;
		Location = loc;
		Velocity = velocity;
	}
}

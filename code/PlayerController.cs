using Godot;
using System;

[GlobalClass]
public abstract partial class PlayerController : RefCounted
{
	static int s_nextAIId = 0;
	public int ID { get; }
	public string Name { get; protected set; }
	public PlayerController Opponent { get; set; }
	public PlayerController()
	{
		ID = ++s_nextAIId;
	}

	public Color MoveColor { get; protected set; }
	public Color GunColor { get; protected set; }

	public abstract void RequestMove(GameBoard gameBoard, Callable callback);
	public abstract void ProcessMove(float timeoutSpan);
}

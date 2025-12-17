using Godot;
using System;

[GlobalClass]
public partial class VisibleUnit : Node2D
{
	private Unit _unit { get; set; } = null;
	private TileMapLayer _map { get; set; } = null;
	private Vector2I _velocity { get; set; } = Vector2I.Zero;
	private Color _color { get; set; } = Colors.White;

	public Unit Unit { get { return _unit; } }

	public void UpdateDots(TileMapLayer map, Vector2I velocity, Color color)
	{
		_map = map;
		_velocity = velocity;
		_color = color;
		QueueRedraw();
	}

	public ModelType GetModel() { return _unit != null ? _unit.Model : null; }

	public void SetUnit(Unit unit)
	{
		_unit = unit;
	}

	public override void _Draw()
	{
		if (_unit == null || _map == null)
		{
			return;
		}

		Vector2 unit_map_pos = _map.MapToLocal(_unit.Location);
		foreach (Vector2I hex in _unit.Model.GetGlobalOccupiedHexes(_unit.Location, Rotation))
		{
			Vector2 hex_map_pos = _map.MapToLocal(hex + _velocity);
			Vector2 hex_global_pos = _map.ToGlobal(hex_map_pos);
			Vector2 hex_local_pos = this.ToLocal(hex_global_pos);
			//Vector2 hex_local_pos = (hex_map_pos - unit_map_pos);
			DrawCircle(hex_local_pos, 5, _color);
		}

		Vector2 destination_map_pos = _map.MapToLocal(_unit.Location + _velocity);
		var unit_local_pos = ToLocal(_map.ToGlobal(unit_map_pos));
		var dest_local_pos = ToLocal(_map.ToGlobal(destination_map_pos));

		DrawDashedLine(unit_local_pos, dest_local_pos, (Colors.White + _color)/2, 1.5f, 8, aligned: true, antialiased: true);
	}
}

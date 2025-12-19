using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class VisibleUnit : Node2D
{
	private Unit _unit { get; set; } = null;
	private TileMapLayer _map { get; set; } = null;
	private Vector2I _velocity { get; set; } = Vector2I.Zero;
	private Color _move_color { get; set; } = Colors.White;
	private Color _gun_color { get; set; } = Colors.Gray;

	private List<Unit> _all_units { get; set; }
	public Unit Unit { get { return _unit; } }

	public void UpdateDots(TileMapLayer map, Vector2I velocity, Color moveColor, Color gunColor, GameBoard gameBoard)
	{
		_map = map;
		_velocity = velocity;
		_gun_color = gunColor;
		_move_color = moveColor;
		_all_units = gameBoard.AllUnits;
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
			DrawCircle(hex_local_pos, 5, _move_color);
		}

		Vector2 destination_map_pos = _map.MapToLocal(_unit.Location + _velocity);
		var unit_local_pos = ToLocal(_map.ToGlobal(unit_map_pos));
		var dest_local_pos = ToLocal(_map.ToGlobal(destination_map_pos));

		DrawDashedLine(unit_local_pos, dest_local_pos, (Colors.White + _move_color) / 2, 1.5f, 8, aligned: true, antialiased: true);

		foreach (Weapon w in Unit.Model.Weapons)
		{
			float range = 200f;
			float startAngle = w.m_rotation - (w.m_totalArc / 2f);
			float endAngle = w.m_rotation + (w.m_totalArc / 2f);
			DrawArc(unit_local_pos, range, startAngle, endAngle, 12, _gun_color);

			Vector2 startPoint = unit_local_pos + Vector2.Right.Rotated(startAngle) * range;
			Vector2 endPoint = unit_local_pos + Vector2.Right.Rotated(endAngle) * range;
			DrawLine(unit_local_pos, startPoint, _gun_color, 0.5f, antialiased: true);
			DrawLine(unit_local_pos, endPoint, _gun_color, 0.5f, antialiased: true);

			foreach (Unit enemy in _all_units.Where(a => a.Owner.Opponent == Unit.Owner))
			{
				if (w.CanHit(_map, Unit, enemy, out float turnNeeded))
				{
					Vector2 enemy_map_local_pos = _map.MapToLocal(enemy.Location);
					Vector2 enemy_global_pos = _map.ToGlobal(enemy_map_local_pos);
					Vector2 enemy_local_pos = this.ToLocal(enemy_global_pos);
					DrawDashedLine(unit_local_pos, enemy_local_pos, _gun_color, 0.5f, dash: 2, aligned: true, antialiased: true);
				}
			}
		}
	}
}

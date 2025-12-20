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

		Vector2 map_local_unit = _map.MapToLocal(_unit.Location);
		foreach (Vector2I grid_hex in _unit.Model.GetGlobalOccupiedHexes(_unit.Location, Rotation))
		{
			Vector2 map_local_unit_hex = _map.MapToLocal(grid_hex + _velocity);
			Vector2 global_unit_hex = _map.ToGlobal(map_local_unit_hex);
			Vector2 visibleunit_local_unit_hex = this.ToLocal(global_unit_hex);
			//Vector2 hex_local_pos = (hex_map_pos - unit_map_pos);
			DrawCircle(visibleunit_local_unit_hex, 5, _move_color);
		}

		Vector2 map_local_destination = _map.MapToLocal(_unit.Location + _velocity);
		var visibleunit_local_unit = ToLocal(_map.ToGlobal(map_local_unit));
		var visibleunit_local_destination = ToLocal(_map.ToGlobal(map_local_destination));

		DrawDashedLine(visibleunit_local_unit, visibleunit_local_destination, (Colors.White + _move_color) / 2, 1.5f, 8, aligned: true, antialiased: true);

		foreach (Weapon w in Unit.Model.Weapons)
		{
			float range = 200f;
			float startAngle = w.m_rotation - (w.m_totalArc / 2f);
			float endAngle = w.m_rotation + (w.m_totalArc / 2f);
			DrawArc(visibleunit_local_unit, range, startAngle, endAngle, 12, _gun_color);

			Vector2I grid_consider_hex = Vector2I.Zero;
			for (float a = -Mathf.Pi; a <= Mathf.Pi; a += Mathf.Pi / 36)
			{
				var ax = range * Mathf.Sin(a) + map_local_unit.X;
				var ay = range * Mathf.Cos(a) + map_local_unit.Y;
				Vector2 map_local_degree_hex = new(ax, ay);
				Vector2I grid_circle_degree_hex = _map.LocalToMap(map_local_degree_hex);
				if (grid_circle_degree_hex.X != grid_consider_hex.X || grid_circle_degree_hex.Y != grid_consider_hex.Y)
				{
					grid_consider_hex = grid_circle_degree_hex;
					Vector2 global_consider_hex = _map.ToGlobal(map_local_degree_hex);
					Vector2 visibleunit_consider_hex = this.ToLocal(global_consider_hex);
					if (w.CanHit(_map, _unit, grid_consider_hex))
					{
						DrawCircle(visibleunit_consider_hex, 2.0f, _gun_color);
					}
				}
			}

			Vector2 startPoint = visibleunit_local_unit + Vector2.Right.Rotated(startAngle) * range;
			Vector2 endPoint = visibleunit_local_unit + Vector2.Right.Rotated(endAngle) * range;
			DrawLine(visibleunit_local_unit, startPoint, _gun_color, 0.5f, antialiased: true);
			DrawLine(visibleunit_local_unit, endPoint, _gun_color, 0.5f, antialiased: true);

			foreach (Unit enemy in _all_units.Where(a => a.Owner.Opponent == Unit.Owner))
			{
				if (w.CanHit(_map, Unit, enemy, out float _turnNeeded, out Vector2I hexHit))
				{
					Vector2 enemy_map_local_pos = _map.MapToLocal(hexHit);
					Vector2 enemy_global_pos = _map.ToGlobal(enemy_map_local_pos);
					Vector2 enemy_local_pos = this.ToLocal(enemy_global_pos);
					float millisecondFraction = 0.8f * DateTime.Now.Millisecond / 1000f;
					Vector2 bulletPath = (enemy_local_pos - visibleunit_local_unit);
					float bulltetPathLength = bulletPath.Length();
					Vector2 bulletPathNorm = bulletPath.Normalized();
					Vector2 bulletStart = visibleunit_local_unit + bulltetPathLength * bulletPathNorm * millisecondFraction;
					Vector2 bulletEnd = visibleunit_local_unit + bulltetPathLength * bulletPathNorm * (0.2f + millisecondFraction);
					DrawDashedLine(bulletStart, bulletEnd, _gun_color, 0.5f, dash: 2, aligned: true, antialiased: true);
				}
			}
		}
	}
}

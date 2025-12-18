class_name GamePlay extends StateMachine

var _game_board : GameBoard
var _current_action : UnitAction
var _players : Array[PlayerController]
var _current_player_index : int = 0

# Map X -> (+16, 0)
# Map Y -> (+8, +12)
@onready var _map_ground : TileMapLayer = $Map_Ground
@onready var _motorcycle_scene : PackedScene = preload("res://scenes/visible_motorcycle.tscn")
@onready var _sidecar_scene : PackedScene = preload("res://scenes/visible_sidecar.tscn")
@onready var _car_scene : PackedScene = preload("res://scenes/visible_car.tscn")
@onready var _camera_target : Marker2D = $Map_Ground/CameraTarget
@onready var _camera : Camera2D = $Map_Ground/CameraTarget/Camera2D
var _initial_center_hex_coord : Vector2i
var _models : Array[VisibleUnit]
@export var _camera_zoom_speed : float = 0.1
var _camera_target_zoom : Vector2 = Vector2.ONE
const _min_zoom : Vector2 = Vector2(0.1, 0.1)
const _max_zoom : Vector2 = Vector2(10, 10)

func RegisterAction(action : UnitAction) -> void:
	_current_action = action
	print("RegisterAction(%s)" % [action.Name])
	switch_state("State_HandlePlayerMove")

const _model_move_speed : float = 45
func UpdateVisibleGameBoard(_delta : float) -> void:
	if _current_action.IsNoOp():
		_advance_to_next_player()
		return
		
	var moving_unit : Unit = _game_board.GetUnitById(_current_action.UnitId)
	var dest_hex : Vector2i = moving_unit.Location + _current_action.Move
	#print("%s moving from %s to %s via %s" % [moving_unit.Name, moving_unit.Location, dest_hex, _current_action.Move])
	var dest_local_pos : Vector2 = _map_ground.map_to_local(dest_hex)
	var travel_dist : float = _model_move_speed * _delta
	for v : VisibleUnit in _models:
		if v.Unit == moving_unit:
			var squared_dist_to = v.position.distance_squared_to(dest_local_pos)
			if squared_dist_to > (travel_dist * travel_dist):
				var mov = (dest_local_pos - v.position).normalized() * travel_dist
				v.position += mov
			else:
				v.position = dest_local_pos
				var nextDest : Vector2i = _map_ground.map_to_local(dest_hex + _current_action.Move)
				v.look_at(nextDest)
				#moving_unit.Update(_current_action)
				_advance_to_next_player()
			return

func _advance_to_next_player() -> void:
	print("Advancing")
	_game_board.ApplyAction(_current_action)
	_current_action = null
	_current_player_index = 1 - _current_player_index
	CenterCamera()
	switch_state("State_RequestPlayerMove")
	
func _input(event: InputEvent) -> void:
	if event.is_pressed():
		if event is InputEventMouseButton:
			if event.button_index == MOUSE_BUTTON_WHEEL_UP:
				_camera_target_zoom *= (1.0 + _camera_zoom_speed)
			elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				_camera_target_zoom *= (1.0 - _camera_zoom_speed)
			_camera_target_zoom = _camera_target_zoom.clamp(_min_zoom, _max_zoom)

func _ready() -> void:
	_game_board = GameBoard.new()
	_game_board.SetMap(_map_ground)
	assert(_game_board != null)
	var screen_center_world = get_viewport().get_visible_rect().get_center()
	var local_coord = _map_ground.to_local(screen_center_world)
	_initial_center_hex_coord = _map_ground.local_to_map(local_coord)
	super._ready()
	var center_hex_local_coords = _map_ground.map_to_local(_initial_center_hex_coord)
	var x_plus_one_local_coords = _map_ground.map_to_local(_initial_center_hex_coord + Vector2i(1, 0))
	print("X Dir = ", str(x_plus_one_local_coords - center_hex_local_coords))
	var y_plus_one_local_coords = _map_ground.map_to_local(_initial_center_hex_coord + Vector2i(-1, 1))
	print("Y Dir = ", str(y_plus_one_local_coords - center_hex_local_coords))

func _process(delta: float) -> void:
	for m : VisibleUnit in _models:
	#	m.rotation += (delta / 3)
		if _current_action != null && not _current_action.IsNoOp():
			if _current_action.UnitId == m.Unit.Id:
				m.UpdateDots(_map_ground, _current_action.Move, Color.PINK)
				continue
		m.UpdateDots(_map_ground, m.Unit.Velocity, Color.GREEN_YELLOW)
	_camera.zoom = _camera.zoom.lerp(_camera_target_zoom, delta)
	pass

func CenterCamera() -> void:
	var total_coords : Vector2 = Vector2.ZERO
	var count : int = 0
	for m : VisibleUnit in _models:
		if m.Unit.IsAlive:
			total_coords += _map_ground.map_to_local(m.Unit.Location + m.Unit.Location + m.Unit.Velocity)
			count += 2
	var hex_to_target : Vector2i = _map_ground.local_to_map(total_coords / count)
	_camera_target.position = _map_ground.map_to_local(hex_to_target)

func RegisterPlayer(pc : PlayerController) -> void:
	assert(_game_board != null)
	assert(_players.size() < 2)
	_players.append(pc)

func RegisterUnit(pc: PlayerController, mt : ModelType, loc : Vector2i, vel : Vector2i) -> void:
	assert(_game_board != null)
	#print("Center Hex Coords: ", _initial_center_hex_coord)
	var unit : Unit = _game_board.AddUnit(pc, mt, loc + _initial_center_hex_coord, vel)
	var visual_model : VisibleUnit
	if mt == ModelType.GetMotorcycle():
		visual_model = _motorcycle_scene.instantiate() # = unit.CreateVisualModel()
	elif mt == ModelType.GetSidecar():
		visual_model = _sidecar_scene.instantiate() # = unit.CreateVisualModel()
	else:
		visual_model = _car_scene.instantiate() # = unit.CreateVisualModel()
	visual_model.position = _map_ground.map_to_local(unit.Location)
	var nextDest : Vector2i = _map_ground.map_to_local(unit.Location + unit.Velocity)
	visual_model.look_at(nextDest)
	visual_model.SetUnit(unit)
	print("Placing %s (%s) at %s (%s) aimed at %s" % [visual_model, unit.Name, visual_model.position, unit.Location, visual_model.rotation])
	_map_ground.add_child(visual_model)
	_models.append(visual_model)

func GetCurrentPlayer() -> PlayerController:
	assert(_current_player_index >= 0 && _current_player_index <= 1)
	assert(_players.size() == 2)
	return _players[_current_player_index]

func GetOtherPlayer() -> PlayerController:
	assert(_current_player_index >= 0 && _current_player_index <= 1)
	assert(_players.size() == 2)
	return _players[1 - _current_player_index]

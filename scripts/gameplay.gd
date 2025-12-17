class_name GamePlay extends StateMachine

var _game_board : GameBoard
var _players : Array[PlayerController]
var _current_player_index : int = 0

# Map X -> (+16, 0)
# Map Y -> (+8, +12)
@onready var _map_ground : TileMapLayer = $Map_Ground
@onready var _motorcycle_scene : PackedScene = preload("res://scenes/visible_motorcycle.tscn")
@onready var _car_scene : PackedScene = preload("res://scenes/visible_car.tscn")
var _initial_center_hex_coord : Vector2i
var _models : Array[Node2D]

func _ready() -> void:
	_game_board = GameBoard.new()
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
	for m : Node2D in _models:
		m.rotation += (delta / 3)
		m.UpdateDots(_map_ground)

func RegisterPlayer(pc : PlayerController) -> void:
	assert(_players.size() < 2)
	_players.append(pc)

func RegisterUnit(pc: PlayerController, mt : ModelType, loc : Vector2i, vel : Vector2i) -> void:
	#print("Center Hex Coords: ", _initial_center_hex_coord)
	var unit : Unit = _game_board.AddUnit(pc, mt, loc + _initial_center_hex_coord, vel)
	var visual_model : VisibleUnit
	if mt == ModelType.GetMotorcycle():
		visual_model = _motorcycle_scene.instantiate() # = unit.CreateVisualModel()
	elif mt == ModelType.GetSidecar():
		visual_model = _motorcycle_scene.instantiate() # = unit.CreateVisualModel()
	else:
		visual_model = _car_scene.instantiate() # = unit.CreateVisualModel()
	visual_model.position = _map_ground.map_to_local(unit.Location)
	var nextDest : Vector2i = _map_ground.map_to_local(unit.Location + unit.Velocity)
	visual_model.look_at(nextDest)
	visual_model.SetUnit(unit)
	print("Placing %s at %s aimed at %s" % [visual_model, visual_model.position, visual_model.rotation])
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

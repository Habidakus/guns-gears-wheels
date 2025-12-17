class_name SMSLoad extends GamePlayStateMachineState

var _running : bool = false;

func enter_state() -> void:
	super.enter_state()
	print("Loading")
	var player_1 : PlayerController = PlayerController_AI.new()
	var player_2 : PlayerController = PlayerController_AI.new()
	GetGamePlay().RegisterPlayer(player_1)
	GetGamePlay().RegisterPlayer(player_2)
	GetGamePlay().RegisterUnit(player_1, ModelType.GetMotorcycle(), Vector2i(-8, 0), Vector2i(10, 0))
	GetGamePlay().RegisterUnit(player_1, ModelType.GetMotorcycle(), Vector2i(-8, 4), Vector2i(10, 0))
	GetGamePlay().RegisterUnit(player_1, ModelType.GetMotorcycle(), Vector2i(-8, -4), Vector2i(10, 0))
	GetGamePlay().RegisterUnit(player_2, ModelType.GetCar(), Vector2i(8, 0), Vector2i(-8, 0))
	_running = true

func exit_state(next_state: StateMachineState) -> void:
	_running = false
	super.exit_state(next_state)

func _process(_delta : float) -> void:
	if _running:
		our_state_machine.switch_state("State_CreateMap")

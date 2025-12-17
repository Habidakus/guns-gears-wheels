class_name SMSLoad extends GamePlayStateMachineState

func enter_state() -> void:
	super.enter_state()
	print("Loading")
	var player_1 : PlayerController = PlayerController_AI.new()
	var player_2 : PlayerController = PlayerController_AI.new()
	GetGamePlay().RegisterPlayer(player_1)
	GetGamePlay().RegisterPlayer(player_2)
	player_1.Opponent = player_2
	player_2.Opponent = player_1
	GetGamePlay().RegisterUnit(player_1, ModelType.GetMotorcycle(), Vector2i(-7, -4), Vector2i(10, 0))
	GetGamePlay().RegisterUnit(player_1, ModelType.GetMotorcycle(), Vector2i(-9, 0), Vector2i(10, 0))
	GetGamePlay().RegisterUnit(player_1, ModelType.GetSidecar(), Vector2i(-11, 4), Vector2i(10, 0))
	GetGamePlay().RegisterUnit(player_2, ModelType.GetCar(), Vector2i(8, 0), Vector2i(0, 8))
	GetGamePlay().CenterCamera()

func _process(_delta : float) -> void:
	if _running:
		our_state_machine.switch_state("State_CreateMap")

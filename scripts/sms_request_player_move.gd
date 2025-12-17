extends GamePlayStateMachineState

var _sent_request : bool = false

func enter_state() -> void:
	_sent_request = false
	super.enter_state()

func _process(_delta: float) -> void:
	if _running:
		if _sent_request == false:
			var game = GetGamePlay()
			var current_player : PlayerController = game.GetCurrentPlayer()
			print("Requesting moves from %s for %s" % [current_player.Name, game._game_board.GetCurrentUnit().Name])
			current_player.RequestMove(game._game_board, Callable(game, "RegisterAction"))
			_sent_request = true
			our_state_machine.switch_state("State_AwaitPlayerMove")

extends GamePlayStateMachineState

func _process(_delta: float) -> void:
	if _running:
		var game = GetGamePlay()
		if game._current_action != null && game._current_action.IsNoOp():
			game._advance_to_next_player()
		else:
			game.GetCurrentPlayer().ProcessMove(_delta)

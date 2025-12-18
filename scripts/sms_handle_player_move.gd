extends GamePlayStateMachineState

func _process(delta: float) -> void:
	if _running:
		GetGamePlay().UpdateVisibleGameBoard(delta)

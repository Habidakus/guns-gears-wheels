extends GamePlayStateMachineState

func _process(delta: float) -> void:
	GetGamePlay().UpdateVisibleGameBoard(delta)

extends GamePlayStateMachineState

func _process(_delta: float) -> void:
	if _running:
		GetGamePlay().GetCurrentPlayer().ProcessMove(_delta)

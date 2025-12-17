extends GamePlayStateMachineState

func enter_state() -> void:
	super.enter_state()
	print("Present Game")

func _process(_delta: float) -> void:
	if _running:
		our_state_machine.switch_state("State_RequestPlayerMove")

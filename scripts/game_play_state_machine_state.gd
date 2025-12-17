class_name GamePlayStateMachineState extends StateMachineState

var _running : bool = false;

func enter_state() -> void:
	super.enter_state()
	_running = true

func exit_state(next_state: StateMachineState) -> void:
	_running = false
	super.exit_state(next_state)

func GetGamePlay() -> GamePlay:
	return our_state_machine as GamePlay

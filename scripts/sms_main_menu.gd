extends StateMachineState

var _gameplay_scene : PackedScene = preload("res://scenes/gameplay_state_machine.tscn")

func _on_play_game_button_up() -> void:
	get_tree().change_scene_to_packed(_gameplay_scene)
	#our_state_machine.switch_state("SplashPage")

extends Node

@onready var audio_stream_player_ui : AudioStreamPlayer = $Player_UI
@onready var audio_stream_player_music : AudioStreamPlayer = $Player_Music
@onready var audio_stream_player_effects : AudioStreamPlayer = $Player_Effects

func play_ui_impact() -> void:
	audio_stream_player_effects.play()

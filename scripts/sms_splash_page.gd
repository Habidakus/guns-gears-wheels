extends StateMachineState_PressAnyKey

@export var label_guns : Label
@export var label_gears : Label
@export var label_wheels : Label

func _ready() -> void:
	_init_label(label_guns)
	_init_label(label_gears)
	_init_label(label_wheels)

func _init_label(label : Label) -> void:
	label.label_settings.font_color = Color(1, 1, 1, 0)
	label.pivot_offset = label_guns.size / 2.0
	label.scale = Vector2(6.0, 6.0)
	label.hide()

func _slam_label(delta : float, label : Label) -> void:
	#delta = clamp( 0, 1, delta)
	var alpha : float = delta
	var s : float = 1.0 + 5.0 * (1.0 - delta)
	#label.show()
	label.label_settings.font_color = Color(1, 1, 1, alpha)
	label.scale = Vector2(s, s)

func exit_state(next: StateMachineState) -> void:
	_init_label(label_guns)
	_init_label(label_gears)
	_init_label(label_wheels)
	super.exit_state(next)

func enter_state() -> void:
	super.enter_state()
	const span : float = 0.5
	var tween : Tween = create_tween()
	tween.tween_callback(Callable(label_guns, "show"))
	tween.tween_method(Callable(self, "_slam_label").bind(label_guns), 0.0, 1.0, span).set_trans(Tween.TransitionType.TRANS_BOUNCE)
	tween.tween_interval(0.5)
	tween.tween_callback(Callable(label_gears, "show"))
	tween.tween_method(Callable(self, "_slam_label").bind(label_gears), 0.0, 1.0, span).set_trans(Tween.TransitionType.TRANS_BOUNCE)
	tween.tween_interval(0.5)
	tween.tween_callback(Callable(label_wheels, "show"))
	tween.tween_method(Callable(self, "_slam_label").bind(label_wheels), 0.0, 1.0, span).set_trans(Tween.TransitionType.TRANS_BOUNCE)
	tween.tween_interval(0.5)

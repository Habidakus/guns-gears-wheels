using Godot;

[GlobalClass]
public partial class ModelType : GodotObject
{
	static ModelType s_motorcycle = new ModelType();
	public static ModelType GetMotorcycle()
	{
		return s_motorcycle;
	}

	static ModelType s_car = new ModelType();
	public static ModelType GetCar()
	{
		return s_car;
	}

	public static ModelType Sidecar;
	public static ModelType Van;
	public static ModelType EighteenWheeler;
}

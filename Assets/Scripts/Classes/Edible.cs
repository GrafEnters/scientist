using System;

[Serializable]
public class Edible
{

	public int RestoreFood;
	public DragEffects dragEffects;

	public enum DragEffects
	{
		None,
		Cafein,
		Cocaine
	}
}

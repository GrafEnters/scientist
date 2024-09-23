using System;
using UnityEngine;

[Serializable]
public class Biome
{
	public string name;
	public int minHeight;
	public Color color;

	public Biome(Biome shablon)
	{
		name = shablon.name;
		minHeight = shablon.minHeight;
		color = shablon.color;
	}
}
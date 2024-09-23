using System;
using UnityEngine;

[Serializable]
public class Matter
{
	public Material material;
	public int id;
	public int Rarity;
	public MoleculeType moleculeType;
	public int Density;
	public int Durability;

	public int BurningTime;
	public int BouncingScale;

	public bool isGlass;
	public bool isMagnet;

	public bool isGlow;

	public bool isFastGrowing;
	public bool isTeleporting;

	public Matter()
	{

	}

	public Matter(Material material, int id, int rarity, MoleculeType moleculeType, int density, int durability, int burningTime, int bouncingScale, bool isGlass, bool isMagnet, bool isGlow, bool isFastGrowing, bool isTeleporting)
	{
		this.material = material;
		this.id = id;
		Rarity = rarity;
		this.moleculeType = moleculeType;
		Density = density;
		Durability = durability;
		BurningTime = burningTime;
		BouncingScale = bouncingScale;
		this.isGlass = isGlass;
		this.isMagnet = isMagnet;
		this.isGlow = isGlow;
		this.isFastGrowing = isFastGrowing;
		this.isTeleporting = isTeleporting;
	}

	public override bool Equals(object obj)
	{
		return obj is Matter matter &&
			   id == matter.id;
	}

	public override int GetHashCode()
	{
		return 1877310944 + id.GetHashCode();
	}
}

public enum MoleculeType
{
	Crystal,
	Fiber
}
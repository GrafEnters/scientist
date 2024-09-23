﻿using UnityEngine;

public interface IBreakable 
{
	void Break(Vector3 hitPoint, int durability);
	void Broken(Vector3 hitPoint);
}

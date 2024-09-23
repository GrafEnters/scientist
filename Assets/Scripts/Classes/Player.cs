using System;
using UnityEngine;

[Serializable]
public class Player
{
	public Vector3 playerPos;
	public Quaternion playerRot;
	public Quaternion camLook;
	public float camLookUtil;
}

using UnityEngine;

public class Tool : Pickable
{
	public virtual void OnPickUp()
	{
		transform.rotation = Quaternion.identity;
	}

	public virtual void OnDrop()
	{

	}
}

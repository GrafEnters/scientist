using UnityEngine;

public class Building : Structure
{
	public LayerMask IgnoreMask;

	public LayerMask CleanLayer;
	public float RadiusToClear;

	public void CleanAround()
	{
		Collider[] colls = Physics.OverlapSphere(transform.position, RadiusToClear, CleanLayer);
		foreach (Collider coll in colls)
		{
			Destroy(coll.gameObject);
		}
	}

	public virtual void Activate(GameObject obj = null)
	{
		Debug.Log(thingName + " активирован");
	}

	public virtual void SideActivate()
	{
		Debug.Log(thingName + " побочно активирован");
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, RadiusToClear);
	}
}

using System.Collections.Generic;
using UnityEngine;

public class Crate : Pickable
{
	public Transform crateCenter;
	public Transform thingsHolder;
	public Transform Hook1;
	public Vector3 halfExtends;
	public LayerMask ignoreMask;
	List<GameObject> inObjects;
	public CrateHook cratehook;

	public Transform hookHolder;

	public Transform checkGroundTransform;
	public LayerMask groundMask;
	public bool isLocked;

	Color normalColor;

	protected override void Start()
	{
		base.Start();
		isLocked = false;
		inObjects = new List<GameObject>();
		normalColor = gameObject.GetComponent<MeshRenderer>().material.GetColor("baseColor");
	}

	public GameObject GetHookObject()
	{
		return cratehook.GetObject();
	}

	public void OnPickUp()
	{
		Collider[] colls = Physics.OverlapBox(crateCenter.position, halfExtends / 2, crateCenter.rotation, ~ignoreMask);
		foreach (Collider coll in colls)
		{
			if (coll.gameObject.GetComponent<Thing>().thingName == "crate")
				continue;
			if (coll.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude < 0.01f)
			{
				inObjects.Add(coll.gameObject);
				coll.gameObject.transform.SetParent(thingsHolder);
				coll.GetComponent<Rigidbody>().isKinematic = true;
				coll.GetComponent<Rigidbody>().detectCollisions = false;
			}
		}
	}

	public void OnDrop()
	{
		foreach (GameObject obj in inObjects)
		{
			obj.transform.SetParent(null);
			obj.GetComponent<Rigidbody>().isKinematic = false;
			obj.GetComponent<Rigidbody>().detectCollisions = true;
			//obj.layer = LayerMask.NameToLayer("Default");
			
		}
		inObjects = new List<GameObject>();
	}

	void ChangeColor(Color color)
	{
		gameObject.GetComponent<MeshRenderer>().material.SetColor("baseColor", color);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(crateCenter.position, halfExtends);
	}

	public bool ChangeLockMode(GameObject handObject = null)
	{
		if(isLocked)
		{
			isLocked = false;
			ChangeColor(normalColor);
			rb.isKinematic = false;
			OnPickUp();
		}
		else
		{
			if (Physics.CheckSphere(checkGroundTransform.position, 0.3f, groundMask))
			{
				isLocked = true;
				ChangeColor(normalColor / 10);
				rb.isKinematic = true;
				OnDrop();
			}
				
		}
		return false;
	}
}

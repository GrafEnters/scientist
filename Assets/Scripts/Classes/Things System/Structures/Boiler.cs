using System.Collections;
using UnityEngine;

public class Boiler : Building, IInteractable
{
	[Header("Boiler")]
	public GameObject matterCubePrefab;
	public Transform In;
	public Vector3 halfExtense;
	public Transform Out;

	/**********/

	private void Start()
	{
		StartCoroutine(ReturnCollider(5));
	}

	private void OnEnable()
	{
		StartCoroutine(ReturnCollider(3));
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(In.position, halfExtense);
		Gizmos.DrawWireCube(Out.position, Vector3.one);
	}

	/**********/

	IEnumerator ReturnCollider(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<MeshCollider>().enabled = true;
	}

	/**********/

	public bool Interact(GameObject handObject = null)
	{
		Collider[] colls = Physics.OverlapBox(In.position, halfExtense / 2, Quaternion.identity, ~IgnoreMask);
		GameObject toBoil = null;
		if (colls.Length > 0)
			toBoil = colls[0].gameObject;


		if (toBoil)
		{
			if (toBoil.transform.tag == "Pickable" && toBoil.GetComponent<Thing>().thingName != "letter")
			{
				Matter matter = toBoil.GetComponent<Thing>().mainM;
				Destroy(toBoil);
				GameObject matterCube = ThingsGenerator.GenerateThingPrefab(matterCubePrefab, matter, null);

				DropResourse(matterCube, Out.position);

			}
		}
		return false;
	}
}

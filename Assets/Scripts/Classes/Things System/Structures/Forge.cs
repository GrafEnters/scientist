using System.Collections.Generic;
using UnityEngine;

public class Forge : Building, IInteractable, IReceivable
{
	[Header("Forge")]
	public Transform Out;
	public float radius;
	public GameObject Trigger;
	public GameObject squarePlace, roundPlace;
	public float rotateSpeed;
	public GameObject[] models;

	GameObject toCreate;
	int curIndex;
	List<GameObject> possibleForms;
	GameObject inObj;

	/**********/

	public void Start()
	{
		possibleForms = new List<GameObject>();
		curIndex = -1;
		for (int i = 0; i < models.Length; i++)
		{
			models[i] = Instantiate(models[i], transform);
			if (models[i].GetComponent<MeshCollider>() != null)
				models[i].GetComponent<MeshCollider>().enabled = false;
			models[i].GetComponent<Rigidbody>().isKinematic = true;
			models[i].SetActive(false);
		}
		Trigger.SetActive(false);
	}

	public void FixedUpdate()
	{
		if (toCreate)
			toCreate.transform.Rotate(Vector3.forward * rotateSpeed);
	}

	private void OnDisable()
	{
		Destroy(toCreate);
		curIndex = -1;
	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(Out.position, Vector3.one / 2);
	}

	/**********/

	public override void Activate(GameObject obj = null)
	{
		base.Activate(obj);
		ForgeForm();
	}
	
	public override void SideActivate()
	{
		base.SideActivate();
		ShowNextPreview();
	}

	public void ForgeForm()
	{
		
		if (toCreate)
		{
			Debug.Log("Произвожу");
			Matter mainM = inObj.GetComponent<Thing>().mainM;
			Matter sideM = inObj.GetComponent<Thing>().mainM;


			GameObject finalProduct = ThingsGenerator.GenerateThing(toCreate, mainM, mainM);
			if (finalProduct.GetComponent<MeshCollider>() != null)
				finalProduct.GetComponent<MeshCollider>().enabled = true;
			DropResourse(finalProduct, Out.position, Quaternion.FromToRotation(Vector3.up, Vector3.forward));
			Destroy(inObj);

			HidePreview();

		}
		else
			Debug.Log("Не выбран крафт");

	}
	
	void HidePreview()
	{
		Destroy(toCreate);
		toCreate = null;
		Trigger.SetActive(false);
		possibleForms.Clear();
	}

	public void ShowNextPreview()
	{
		if (possibleForms.Count > 0)
		{
			Trigger.SetActive(true);
			curIndex++;
			curIndex %= possibleForms.Count;

			Destroy(toCreate);
			toCreate = Instantiate(possibleForms[curIndex], Out.position, Quaternion.FromToRotation(Vector3.up, Vector3.forward), transform);
			toCreate.SetActive(true);
			toCreate.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
		
	}

	public void Upgrade()
	{
		squarePlace.SetActive(false);
		roundPlace.SetActive(true);
	}
	   
	/**********/

	public bool Interact(GameObject handObject = null)
	{
		SideActivate();
		return false;
	}

	public void OnPlaced(GameObject placedObj, int index = 0)
	{
		inObj = placedObj;
		curIndex = -1;
		switch (inObj.GetComponent<Thing>().thingName)
		{
			case "matterCube":
				possibleForms.Add(models[0]);// Cylinder
				possibleForms.Add(models[1]);// Tetraedr
				possibleForms.Add(models[2]);// Plank
				break;
			case "cylinder":
				possibleForms.Add(models[3]);// Axis
				possibleForms.Add(models[4]);// Tube
				possibleForms.Add(models[6]);// Holer
				break;
			case "tetraedr":
				possibleForms.Add(models[5]);// Pyramid
				break;
			case "plank":

				break;
		}
		ShowNextPreview();
	}

	public void OnRemoved(int index = 0)
	{
		inObj = null;
		curIndex = -1;
		possibleForms.Clear();
		HidePreview();
	}
}

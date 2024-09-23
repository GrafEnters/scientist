using UnityEngine;

public class Workbench : Building, IReceivable
{
	[Header("Workbench")]
	public Transform Out;
	public Transform In1, In2;
	public float radius;
	public GameObject[] models;
	
	public GameObject Trigger;
	public float rotateSpeed;

	int curIndex;
	GameObject toCreate;
	Matter finalMain, finalSide;
	GameObject obj1, obj2;

	/**********/

	private void Start()
	{
		Trigger.SetActive(false);
	}

	void FixedUpdate()
	{
		if (toCreate)
			toCreate.transform.Rotate(Vector3.up * rotateSpeed);

	}

	/**********/

	public override void Activate(GameObject obj = null)
	{
		if (toCreate)
		{

			GameObject product = ThingsGenerator.GenerateThing(toCreate, finalMain, finalSide);

			product.transform.position = Out.transform.position;
			product.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			product.layer = LayerMask.GetMask("Default");
			Trigger.SetActive(false);

			Destroy(toCreate);
			Destroy(obj1);
			Destroy(obj2);

		}

	}

	GameObject MakeFrom(GameObject a, GameObject b)
	{
		string nameA = a.GetComponent<Thing>().thingName, nameB = b.GetComponent<Thing>().thingName;
		Matter matterA = a.GetComponent<Thing>().mainM, matterB = b.GetComponent<Thing>().mainM;
		finalMain = matterA; finalSide = matterB;
		//Debug.Log(nameA + "   " + nameB);
		if (nameA == "cylinder" && nameB == "tetraedr" || nameB == "cylinder" && nameA == "tetraedr")
		{
			if (nameA == "cylinder")
				Swap();


			return models[0]; // топор
		}

		if (nameA == "cylinder" && nameB == "pyramid" || nameB == "cylinder" && nameA == "pyramid")
		{
			if (nameA == "cylinder")
				Swap();
			return models[1]; // кирка
		}

		if (nameA == "tube" && nameB == "pyramid" || nameB == "tube" && nameA == "pyramid")
		{
			if (nameA == "tube")
				Swap();
			return models[2]; // экструдер
		}

		if (nameA == "tetraedr" && nameB == "axis" || nameB == "tetraedr" && nameA == "axis")
		{
			if (nameA == "axis")
				Swap();
			return models[3]; // основа весов
		}

		if (nameA == "matterCube" && nameB == "cylinder" || nameB == "matterCube" && nameA == "cylinder")
		{
			if (nameA == "cylinder")
				Swap();
			return models[4]; // молот
		}

		if (nameA == "holer" && nameB == "plank" || nameB == "holer" && nameA == "plank")
		{
			if (nameA == "holer")
				Swap();
			return models[5]; // holed plank
		}
		return null;
	}

	void Swap()
	{
		Matter tmp = finalMain;
		finalMain = finalSide;
		finalSide = tmp;
	}

	/**********/

	public void OnPlaced(GameObject placedObj, int index = 0)
	{
		if (index == 0)
			obj1 = placedObj;

		if (index == 1)
			obj2 = placedObj;

		if (obj1 && obj2)
		{
			toCreate = MakeFrom(obj1, obj2);
			if (toCreate)
			{
				toCreate = Instantiate(toCreate, Out.transform.position, Quaternion.FromToRotation(Vector3.up, Vector3.forward + Vector3.up));
				toCreate.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				toCreate.layer = 2; // ignoreRaycast
				toCreate.transform.position = Out.position;
				Trigger.SetActive(true);
			}
		}


	}

	public void OnRemoved(int index = 0)
	{
		if (index == 0)
			obj1 = null;
		if (index == 1)
			obj2 = null;

		Trigger.SetActive(false);
		Destroy(toCreate);
		toCreate = null;
	}
}

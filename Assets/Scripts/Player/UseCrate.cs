using UnityEngine;

public class UseCrate : MonoBehaviour
{
	GameObject crate;
	GameObject onHookTool;

	public void TakeCrate(GameObject crateObj)
	{
		crate = crateObj;
		crate.SetActive(false);
		crate.transform.SetParent(gameObject.transform);
		onHookTool = crate.GetComponent<Crate>().GetHookObject();
	}

	public GameObject UnhideCrate()
	{
		if (crate)
		{
			GameObject tmp = crate;
			

			tmp.SetActive(true);
			tmp.transform.SetParent(gameObject.transform);
			tmp.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2 + Vector3.down;
			tmp.transform.rotation = Quaternion.identity;
			tmp.GetComponent<Rigidbody>().mass = 1000;				
			tmp.GetComponent<Crate>().cratehook.Interact(onHookTool);

			crate = null;
			onHookTool = null;

			return tmp;

		}
		return null;
	}

	public GameObject SwitchItems(GameObject holdObject)
	{
		if (crate)
		{
			GameObject tmp = onHookTool;
			if (tmp)
				tmp.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2 + Vector3.down;

			onHookTool = holdObject;
			if (onHookTool)
				onHookTool.SetActive(false);

			return tmp;
		}

		return holdObject;
	}
}

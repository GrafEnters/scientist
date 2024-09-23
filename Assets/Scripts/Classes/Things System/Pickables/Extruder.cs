using UnityEngine;

public class Extruder : Tool
{
	public GameObject SeedModel;

	public override bool SideUse()
	{
		
		ExtrudeSeed();
		return true;
	}

	void ExtrudeSeed()
	{

		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
		if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: Hold.maxOperateDistance))
		{
			GameObject fruit = hit.collider.GetComponent<Transform>().gameObject;

			if (fruit.GetComponent<Fruit>())
			{
				Fruit fruitScript = fruit.GetComponent<Fruit>();
				fruitScript.Rot();
				return;
			}
			else
				Debug.Log("Это не фрукт");

		}
	}
}

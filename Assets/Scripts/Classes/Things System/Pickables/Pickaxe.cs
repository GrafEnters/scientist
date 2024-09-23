using UnityEngine;

public class Pickaxe : Tool
{
	public override bool Use()
	{
		DestroyCrystal();
		return true;
	}

	void DestroyCrystal()
	{
		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
		if (Physics.Raycast(ray, out RaycastHit hit, Hold.maxOperateDistance))
		{
			GameObject objToBreak = hit.collider.gameObject;

			if (objToBreak.GetComponent<Thing>() is IBreakable && objToBreak.GetComponent<Thing>().mainM.moleculeType == MoleculeType.Crystal)
			{
				objToBreak.GetComponent<IBreakable>().Break(hit.point, mainM.Durability);
				return;
			}
		}
	}

}

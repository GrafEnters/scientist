using UnityEngine;

public class Axe : Tool
{
	public override bool Use()
	{
		DestroyFiber();
		return true;
	}

	void DestroyFiber()
	{
		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
		if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: Hold.maxOperateDistance))
		{
			GameObject objToBreak = hit.collider.gameObject;

			// Если структура - разрушаем
			if (objToBreak.GetComponent<Thing>() is IBreakable && objToBreak.GetComponent<Thing>().mainM.moleculeType == MoleculeType.Fiber)
			{
				objToBreak.GetComponent<IBreakable>().Break(hit.point, mainM.Durability);
				return;
			}
		}
	}
}

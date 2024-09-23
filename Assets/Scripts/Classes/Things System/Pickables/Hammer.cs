using UnityEngine;

public class Hammer : Tool
{
	public override bool Use()
	{
		KickTree();
		return true;
	}

	void KickTree()
	{
		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
		if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: Hold.maxOperateDistance))
		{
			GameObject objToBreak = hit.collider.gameObject;

			// Если структура - разрушаем
			if (objToBreak.GetComponent<Tree>() )
			{
				objToBreak.GetComponent<Tree>().KickedDrofFruit(hit.point, mainM.Durability);
				return;
			}
		}
	}
}

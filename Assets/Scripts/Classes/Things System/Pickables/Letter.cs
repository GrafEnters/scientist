using UnityEngine;

public class Letter : Pickable
{
	public GameObject TriangleSign, CircleSign, SquareSign;
	Task task;

	public override bool Use()
	{
		LettersHandler.ShowLetter(task.text, task.name);
		return false;
	}

	public override bool SideUse()
	{
		GiveLetterBack();
		return true;
	}

	public void SetTask(Task _task)
	{
		task = _task;
		if (task.taskType == TaskType.observe)
		{
			TriangleSign.SetActive(true);
			Destroy(CircleSign);
			Destroy(SquareSign);
		}
		else if(task.taskType == TaskType.bring)
		{
			CircleSign.SetActive(true);
			Destroy(TriangleSign);
			Destroy(SquareSign);
		}
		else if (task.taskType == TaskType.make)
		{
			SquareSign.SetActive(true);
			Destroy(TriangleSign);
			Destroy(CircleSign);
		}

	}

	public Task GetTask()
	{
		return task;
	}

	void GiveLetterBack()
	{
		int x = Screen.width / 2;
		int y = Screen.height / 2;
		Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));
		if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: Hold.maxOperateDistance))
		{
			GameObject villager = hit.collider.GetComponent<Transform>().gameObject;

			
			if (villager.GetComponent<Villager>() != null)
			{
				villager.GetComponent<Villager>().TakeLetter(gameObject);

			}
		}
	}


}

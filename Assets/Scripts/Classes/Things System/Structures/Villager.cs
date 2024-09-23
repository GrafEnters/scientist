using UnityEngine;

public class Villager : Building, IInteractable
{	
	public float pickUpRadius;
	[HideInInspector]
	public GameObject signPost;

	protected GameObject curLetter;

	public virtual void TakeLetter(GameObject letter)
	{
		Task task = letter.GetComponent<Letter>().GetTask();
		if (task.villager == this)
		{
			curLetter = letter;					
			task.CheckConditionsMet(task);				
		}
		else
		{
			TalkManager.Say("Не моё!");
		}
	}

	public virtual bool Interact(GameObject handObject = null)
	{
		Debug.Log(" я интерактабл");
		return false;
	}

	public virtual void CheckConditionsMet(Task task)
	{
		VillagerAnswer(false, 0,"Не подходит");
	}

	protected virtual void VillagerAnswer(bool isCorrect, int ChangeRep, string phraze)
	{
		signPost.GetComponent<SignPost>().ChangerReputation(ChangeRep);
		TalkManager.Say(phraze);
	}

	protected virtual Task GenerateTask()
	{
		return new Task
		{
			taskType = TaskType.observe,
			text = "текст задания от жителя,",
			answer = "ответ",
			name = "житель",
			villager = this
		};

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, pickUpRadius);
	}

}

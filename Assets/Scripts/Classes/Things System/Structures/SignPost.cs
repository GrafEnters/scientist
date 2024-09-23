using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignPost : Building
{
	[Header("SignPost")]
	public Transform[] taskPositions;
	public GameObject LetterModel;
	public GameObject Letter;
	public float WaitBetweenChecksInSeconds = 5;
	public TextMesh reputationPointsText;


	Queue<Task> tasksQ;	
	int reputation;

	/**********/

	private void Awake()
	{
		tasksQ = new Queue<Task>();
		reputation = 0;
	}

	private void OnEnable()
	{
		StartCoroutine(TaskAvailableCheck());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	/**********/

	public override void Init(int id, Matter main, Matter side)
	{
		base.Init(id, main, side);
		Letter = ThingsGenerator.GenerateThingPrefab(LetterModel,null,null,0,MoleculeType.Fiber);
	}

	public void EnqueTask(Task task)
	{
		tasksQ.Enqueue(task);
	}

	public void ChangerReputation(int delta)
	{
		reputation += delta;
		reputationPointsText.text = reputation.ToString();
	}

	void SpawnTask(Task task, Transform place)
	{
		GameObject taskObj = Instantiate(Letter, place.position, transform.rotation* Quaternion.Euler(Vector3.up * 180), place);

		taskObj.SetActive(true);
		taskObj.GetComponent<Letter>().SetTask(task);
		taskObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
	}

	IEnumerator TaskAvailableCheck()
	{
		for (; ; )
		{
			for (int i = 0; i < taskPositions.Length; i++)
			{
				if (taskPositions[i].childCount == 0 && tasksQ.Count > 0)
				{
					SpawnTask(tasksQ.Dequeue(), taskPositions[i]);
				}
			}
			yield return new WaitForSeconds(WaitBetweenChecksInSeconds);
		}

	}
	
}

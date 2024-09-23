using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Plant : NatureStructure
{
	[HideInInspector]
	public GameObject ItPrefab;
	public float liveStepMax = 5, liveTimeMax = 100;

	protected float liveTime;
	protected float curScale, liveStep; // от 0.4 до 2, потом умирает

	public override void Init(int id, Matter main, Matter side)
	{
		base.Init(id, main, side);
		liveTime = 0;
		tempRadius = 20;
		ItPrefab = gameObject;
		if (isDecorative)		
			tempRadius = 40;			
	}

	private void Start()
	{
		liveTime = Random.Range(0, 100f);   // сколько лет в процентах от макс		
		liveStep = Random.Range(0, liveStepMax);
	}

	protected virtual void Grow()
	{
		//Debug.Log("Growned");		
	}

	protected void OverGrown()
	{
		//Debug.Log("Overgrowned");
		Destroy(gameObject);
	}

	private void OnEnable()
	{
		Grow();
		StartCoroutine(GrowCoroutine());
	}

	private void OnDisable()
	{
		//StopCoroutine(GrowCoroutine());
	}

	IEnumerator GrowCoroutine()
	{
		while (true)
		{
			liveTime += Time.deltaTime;
			liveStep += Time.deltaTime;
			if (liveStep >= liveStepMax)
			{
				liveStep = 0;
				Grow();
			}
			yield return new WaitForEndOfFrame();
		}
	}
}

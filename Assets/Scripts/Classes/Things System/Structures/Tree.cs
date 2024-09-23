using UnityEngine;

public class Tree : Plant
{
	public int maxFruitsAmount = 3;

	public GameObject FruitModel;

	[HideInInspector]
	public GameObject FruitPrefab;

	public Transform LogPlace;
	public Transform FruitPlace;

	public float FruitPlaceRadius = 1;

	public override void Init(int id, Matter main, Matter side)
	{
		base.Init(id, main, side);
		liveStepMax = Random.Range(25, 50);
		liveTimeMax = Random.Range(250, 500);

		FruitPrefab = ThingsGenerator.GenerateThingPrefab(FruitModel, null, main, main.Rarity, main.moleculeType);
		FruitPrefab.GetComponent<Fruit>().GetDadandSon(gameObject);
	}

	private void Start()
	{
		curScale = 0.4f + (liveTime / 100f) * 0.6f;
		transform.localScale = Vector3.one * curScale;

	}


	protected override void Grow()
	{
		base.Grow();
		curScale = 0.4f + (liveTime / 100f) * 0.6f;
		if (curScale >= 0.8f)
			DropFruit();
		transform.localScale = Vector3.one * curScale;
		if (curScale <= 1)
		{
		}
		else if (Random.Range(1f, 2f) < curScale)
			OverGrown();
		else if (dropPrefab.Length > 0)
			DropFruit();
	}

	void DropFruit()
	{
		if (FruitPlace)
			DropResourse(FruitPrefab, FruitPlace.position);
		else
			DropResourse(FruitPrefab, transform.position);
	}

	public void KickedDrofFruit(Vector3 hitPoint, int durability)
	{
		
		if (Random.Range(0, 4) == 0)
			Break(hitPoint, durability);
		else if (Random.Range(0,3) == 0) 
				DropFruit();
	}

	public override void Break(Vector3 hitPoint, int durability)
	{
		base.Break(hitPoint, durability);
	}

	public override void Broken(Vector3 hitpoint)
	{
		if (LogPlace)
		{
			GameObject log = DropResourse(dropPrefab[0], LogPlace.position, LogPlace.rotation);
			log.transform.localScale *= transform.localScale.x;
		}
		else
			DropResourse(dropPrefab[0], transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(FruitPlace.position, FruitPlaceRadius);
	}
}

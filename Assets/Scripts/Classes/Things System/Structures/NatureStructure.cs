using UnityEngine;

public class NatureStructure : Structure, IBreakable
{
	public bool isDecorative;
	public bool isRock;
	public float maxAngle = 361;
	public int cost = 10;
	public int prolEdge, idealTemp,  tempRadius = 5;
	public int minimumDustanceFromSpawn = 0;
	public float spaceRadius;
	public LayerMask spaceMask;

	public GameObject[] dropModel;
	[HideInInspector]
	public GameObject[] dropPrefab;

	public override void Init(int id, Matter main, Matter side)
	{
		base.Init(id, main, side);

		dropPrefab = new GameObject[dropModel.Length];
		for (int i = 0; i < dropModel.Length; i++)
		{
			dropPrefab[i] = ThingsGenerator.GenerateThingPrefab(dropModel[i], main, side);
		}

		if (mainM.Rarity == 0)
			minimumDustanceFromSpawn = 0;
		if (mainM.Rarity == 1)
			minimumDustanceFromSpawn = 50;
		if (mainM.Rarity == 2)
			minimumDustanceFromSpawn = 150;
		if (mainM.Rarity == 3)
			minimumDustanceFromSpawn = 1500;
		if (mainM.Rarity == 4)
			minimumDustanceFromSpawn = 3000;


		idealTemp = Random.Range(0, 100);
		tempRadius = 40;
	}

	private void Start()
	{
		if (!CheckSpaceAround())
			Destroy(gameObject);
		hp = 1000;
	}

	public virtual void Break(Vector3 hitPoint, int durability)
	{
		if(mainM.Durability - durability < 20)
		{
			if (hp == 1000)
			{
				hp = mainM.Durability - durability;
				if (hp <= 0)
					hp = 3;
			}

			hp--;
			if (hp <= 0)
			{
				Broken(hitPoint);
			}
		}
		
	}

	public virtual void Broken(Vector3 hitpoint)
	{
		Destroy(gameObject);
		DropResourse(dropPrefab[0], hitpoint);
		DropResourse(dropPrefab[0], hitpoint);
	}

	public virtual bool CheckWeatherComfort(float distancefromspawn, int temp, int prol)
	{
		if(isRock)
			return distancefromspawn > minimumDustanceFromSpawn && prol < prolEdge && Mathf.Abs(temp - idealTemp) < tempRadius;
		else
			return distancefromspawn > minimumDustanceFromSpawn && prol > prolEdge && Mathf.Abs(temp - idealTemp) < tempRadius;
	}

	public virtual bool CheckSpaceAround()
	{
		Collider[] colls;
		int layer = gameObject.layer;
		gameObject.layer = 0;
		colls = Physics.OverlapSphere(transform.position, spaceRadius, spaceMask);
		gameObject.layer = layer;

		if (colls.Length > 0)
			return false;
		return true;

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, spaceRadius);
	}
}

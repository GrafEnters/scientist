using System.Collections;
using UnityEngine;

public class Fruit : Pickable, IEdible
{
	public GameObject parentStructure;
	public GameObject SeedModel;

	public GameObject SeedPrefab;
	public Edible edible;

	public float GroundCheckRadius;
	public float rotTime = 60;
	float rotRemainTime;
	public override void Init(int id, Matter main, Matter side)
	{
		base.Init(id, main, side);
		edible = new Edible
		{
			RestoreFood = Random.Range(1, 50),
			dragEffects = Random.Range(0, 4) != 0 ? Edible.DragEffects.None : Random.Range(0, 2) == 0 ? Edible.DragEffects.Cafein : Edible.DragEffects.Cocaine //Мдэ, надо переписать
		};
	}


	public void GetDadandSon(GameObject dad)
	{
		parentStructure = dad;
		SeedPrefab = ThingsGenerator.GenerateThingPrefab(SeedModel, parentStructure.GetComponent<Thing>().mainM, parentStructure.GetComponent<Thing>().sideM);
		SeedPrefab.GetComponent<Seed>().parentStructure = parentStructure;
	}


	public void Rot()
	{
		GameObject seed = Instantiate(SeedPrefab, transform.position, Quaternion.identity);
		seed.GetComponent<Rigidbody>().isKinematic = false;
		seed.GetComponent<Rigidbody>().useGravity = true;
		seed.SetActive(true);
		seed.transform.SetParent(null);
		Destroy(gameObject);
	}

	protected override void Start()
	{
		base.Start();
		rotRemainTime = rotTime;
	}

	private void OnEnable()
	{
		StartCoroutine(Rotting());
		//Debug.Log("start Rotting");
	}

	IEnumerator Rotting()
	{
		while (rotRemainTime >= 0)
		{
			yield return new WaitForSeconds(1);

			if (GetComponent<Rigidbody>().linearVelocity.sqrMagnitude > 0.01f)
				rotRemainTime = rotTime;

			LayerMask mask = LayerMask.GetMask("Ground");
			if (Physics.CheckSphere(transform.position, GroundCheckRadius * transform.localScale.x, mask))
				rotRemainTime -= 1;
			else
				rotRemainTime = rotTime;
		}
		Rot();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, GroundCheckRadius * transform.localScale.x);
	}

	public Edible GetEdible()
	{
		return edible;
	}
}

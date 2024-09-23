using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int maxHealth = 100, maxFood = 100;
	public float starvingPerSecond = 0.5f;
	int curHealth, curFood;
	float curFoodStep;
	private void Start()
	{
		curHealth = maxHealth;
		curFood = maxFood;
		StartCoroutine(Starving());
	}

	public void Eat(IEdible obj)
	{
		Edible edible = obj.GetEdible();
		curFood += edible.RestoreFood;
		switch (edible.dragEffects)
		{
			case Edible.DragEffects.Cafein:
				StartCoroutine(OnCafein());
				break;
			case Edible.DragEffects.Cocaine:
				StartCoroutine(OnCocaine());
				break;
		}
	}

	IEnumerator Starving()
	{
		for(; ;)
		{
			curFoodStep += starvingPerSecond;
			if(curFoodStep >= 1)
			{
				curFood -= (int)curFoodStep;
				curFoodStep = 0;
			}
			//Debug.Log("Текущий голод: " + curFood);
			yield return new WaitForSeconds(1);
		}
	}


	public IEnumerator OnCafein()
	{
		yield return new WaitForSeconds(5);
	}

	public IEnumerator OnCocaine()
	{
		Time.timeScale *= 2;
		yield return new WaitForSeconds(5);
		Time.timeScale /= 2;
	}
}

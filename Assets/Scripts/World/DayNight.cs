	using UnityEngine;

	public class DayNight : MonoBehaviour
{
	public Transform DirectionalLigth;
	public Light lightComp;
	[Range(0, 1)]
	public float NigthBeginInPercent, MorningBeginInPercent;
	int dayLength;
	float dayTime;
	[Range(0, 1)]
	public float percent;

	/**********/

	private void Start()
	{
		dayLength = TimeManager.GetDayLength();
	}

	private void Update()
	{
		dayTime = TimeManager.GetDayTime();
		SetSun();
	}

	void SetSun()
	{
		percent = (dayTime + 0.0f) / dayLength;
		DirectionalLigth.localRotation = Quaternion.Euler(90 + (percent * 360f), 170, 0);
		if (percent > NigthBeginInPercent && percent < MorningBeginInPercent)
			lightComp.intensity = 0;
		else
			lightComp.intensity = 1;

		/*
		float angularVelocity = 2 * Mathf.PI / dayLength * 60;
		DirectionalLigth.RotateAround(Vector3.zero, Vector3.right, angularVelocity * Time.deltaTime);
		DirectionalLigth.LookAt(Vector3.zero);
		*/
	}

}

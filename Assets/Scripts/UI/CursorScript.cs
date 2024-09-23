using UnityEngine;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
	public Sprite[] icons;
	Image centerImage;

	private void Start()
	{
		centerImage = GetComponent<Image>();
	}

	public void ChangeIcon(Center_Icons icon)
	{
		if (icon == Center_Icons.Dot)
			centerImage.sprite = icons[0];
		else if (icon == Center_Icons.Hand)
			centerImage.sprite = icons[1];
		else if (icon == Center_Icons.Gear)
			centerImage.sprite = icons[2];
	}
}
public enum Center_Icons
{
	Dot,
	Hand,
	Gear
}

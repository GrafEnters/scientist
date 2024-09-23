using UnityEngine;
using UnityEngine.UI;

public class LettersHandler : MonoBehaviour
{
	public GameObject SLetter;
	public Text SletterText, SletterName;
	public static GameObject letter;
	public static Text letterText, letterName;
	

	private void Start()
	{
		letter = SLetter;
		letterText = SletterText;
		letterName = SletterName;
	}

	public static void ShowLetter(string text, string name)
	{
		letter.SetActive(true);
		letterText.text = text;
		letterName.text = name;
		Time.timeScale = 0;

	}

	void Update()
	{
		if (letter.activeSelf && Input.anyKeyDown)
			Hide();
	}	

	public void Hide()
	{
		Time.timeScale = 1;
		letter.SetActive(false);
	}
}

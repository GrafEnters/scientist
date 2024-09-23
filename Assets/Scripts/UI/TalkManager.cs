using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
	public static Text Text;
	public static InputField AskField;
	public static TalkManager TM;
	public static UnityAction<string> CallBackMethod;

	private void Awake()
	{
		Text = GetComponent<Text>();
		TM = GetComponent<TalkManager>();
		AskField =  GameObject.Find("Ask InputField").GetComponent<InputField>();
		AskField.gameObject.SetActive(false);
		CallBackMethod = null;
		AskField.onEndEdit.AddListener(EnteredAnswer);
	}

	public static void Say(string phraze)
	{
		Text.text = phraze;
		TM.StopAllCoroutines();
		TM.StartCoroutine(ShowPhraze());
	}

	public static void Ask(UnityAction<string> method)
	{
		InputManager.CanPlayerMove = false;
		AskField.gameObject.SetActive(true);
		AskField.Select();
		Time.timeScale = 0;
		CallBackMethod = method;
	}

	public static void EnteredAnswer(string answer)
	{
		AskField.gameObject.SetActive(false);

		Time.timeScale = 1;
		CallBackMethod(answer);

		InputManager.CanPlayerMove = true;
		AskField.text = "";
		CallBackMethod = null;	
	}


	public static IEnumerator ShowPhraze()
	{
		Text.color = Color.white;
		float time = 7.5f;
		while (time > 0)
		{
			time -= Time.deltaTime;
			if (time < 0)
				time = 0;
			Text.color = new Color(255, 255, 255, time / 7.5f);
			yield return new WaitForEndOfFrame();
		}
		Text.text = "";
	}
}

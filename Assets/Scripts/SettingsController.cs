using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
	public MusicScript musicScript;
	Button playButton, pauseButton, nextButton, closeButton;
	UIDocument mainDoc;

	// Start is called before the first frame update
	void Start()
	{
		mainDoc = GetComponent<UIDocument>();

		playButton = mainDoc.rootVisualElement.Q("playMusic") as Button;
		pauseButton = mainDoc.rootVisualElement.Q("pauseMusic") as Button;
		nextButton = mainDoc.rootVisualElement.Q("nextMusic") as Button;

		playButton.RegisterCallback<ClickEvent>((_) => musicScript.StartStop());
		pauseButton.RegisterCallback<ClickEvent>((_) => musicScript.Pause());
		nextButton.RegisterCallback<ClickEvent>((_) => musicScript.Next());

		closeButton = mainDoc.rootVisualElement.Q("close") as Button;
		closeButton.RegisterCallback<ClickEvent>((_) => gameObject.SetActive(false));
	}
}
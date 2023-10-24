using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
	public MusicScript musicScript;
	public SoundEffects sfxScript;
	
	UIDocument mainDoc;
	Button startButton, pauseButton, nextButton, closeButton;
	Label trackLabel;
	Slider musicSlider, sfxSlider;

	// Start is called before the first frame update
	void Awake()
	{
		mainDoc = GetComponent<UIDocument>();
	}

	private void OnEnable()
	{
		startButton = mainDoc.rootVisualElement.Q("startMusic") as Button;
		pauseButton = mainDoc.rootVisualElement.Q("pauseMusic") as Button;
		nextButton = mainDoc.rootVisualElement.Q("nextTrack") as Button;
		closeButton = mainDoc.rootVisualElement.Q("close") as Button;
		trackLabel = mainDoc.rootVisualElement.Q("track") as Label;
		musicSlider = mainDoc.rootVisualElement.Q("musicVolume") as Slider;
		sfxSlider = mainDoc.rootVisualElement.Q("sfxVolume") as Slider;

		UpdateUI();

		startButton.RegisterCallback<ClickEvent>((_) => StartMusic());
		pauseButton.RegisterCallback<ClickEvent>((_) => PauseMusic());
		closeButton.RegisterCallback<ClickEvent>((_) => CloseSettings());
		nextButton.RegisterCallback<ClickEvent>((_) => NextTrack());
		musicSlider.RegisterCallback<ChangeEvent<float>>(ChangeMusicVolume);
		sfxSlider.RegisterCallback<ChangeEvent<float>>(ChangeSFXVolume);
	}
	private void OnDisable()
	{
		startButton.UnregisterCallback<ClickEvent>((_) => StartMusic());
		pauseButton.UnregisterCallback<ClickEvent>((_) => PauseMusic());
		closeButton.UnregisterCallback<ClickEvent>((_) => CloseSettings());
		nextButton.UnregisterCallback<ClickEvent>((_) => NextTrack());
		musicSlider.UnregisterCallback<ChangeEvent<float>>(ChangeMusicVolume);
		sfxSlider.UnregisterCallback<ChangeEvent<float>>(ChangeSFXVolume);
	}

	void StartMusic()
	{
		musicScript.StartStop();
		UpdateUI();
	}
	void PauseMusic()
	{
		musicScript.Pause();
		UpdateUI();
	}
	void NextTrack()
	{
		musicScript.Next();
		UpdateUI();
	}
	void ChangeMusicVolume(ChangeEvent<float> e)
	{
		musicScript.SetVolume(e.newValue);
		UpdateUI();
	}
	void ChangeSFXVolume(ChangeEvent<float> e)
	{
		sfxScript.SetVolume(e.newValue);
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (musicScript.musicPlaying)
		{
			startButton.text = "Stop";

			pauseButton.SetEnabled(true);
			pauseButton.text = musicScript.musicPaused ? "Folytat" : "Szüneteltet";

			trackLabel.text = musicScript.GetCurrentTrack();
		}
		else
		{
			startButton.text = "Lejátszás";

			pauseButton.SetEnabled(false);
			pauseButton.text = "Szüneteltet";
			
			trackLabel.text = string.Empty;
		}
		musicSlider.value = musicScript.GetVolume();
		sfxSlider.value = sfxScript.GetVolume();
	}

	public void CloseSettings()
	{
		gameObject.SetActive(false);
	}
}
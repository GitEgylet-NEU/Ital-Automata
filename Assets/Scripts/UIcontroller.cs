using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
public class UIcontroller : MonoBehaviour
{
	public static UIcontroller instance;
	private void Awake()
	{
		instance = this;
	}

	public Dispenser dispenser;
	public GyroscopeHandler gyroHandler;
	public GameObject settingsMenu;
	UIDocument mainDoc;

	//ui elements
	[HideInInspector] public Button calculateButton, gyroButton;
	Button settingsButton;
	Label errorLabel, litersLabel;
	TextField timeField, speedField, diameterField;
	ScrollView colours;
	VisualElement renderWindow;

	//events
	public UnityEvent<int> onErrorStateChanged;
	public UnityEvent<float> onSpeedChanged, onTimeChanged, onDiameterChanged;
	public UnityEvent onCalculateButtonPressed, onGyroButtonPressed;
	void Start()
	{
		onErrorStateChanged.AddListener(HandleError);

		mainDoc = GetComponent<UIDocument>();

		//button
		calculateButton = mainDoc.rootVisualElement.Q("calculate") as Button;
		calculateButton.RegisterCallback<ClickEvent>(Calculate);

		gyroButton = mainDoc.rootVisualElement.Q("gyroscope") as Button;
		gyroButton.RegisterCallback<ClickEvent>((_) => onGyroButtonPressed.Invoke());
		if (!SystemInfo.supportsGyroscope)
		{
			gyroButton.style.unityBackgroundImageTintColor = Color.yellow;
			Debug.Log(gyroButton.style.unityBackgroundImageTintColor);
			gyroButton.SetEnabled(false);
		}

		settingsButton = mainDoc.rootVisualElement.Q("settings") as Button;
		settingsButton.RegisterCallback<ClickEvent>(ToggleSettings);

		//text
		litersLabel = mainDoc.rootVisualElement.Q("liters") as Label;
		errorLabel = mainDoc.rootVisualElement.Q("errorLabel") as Label;

		//inpufield
		speedField = mainDoc.rootVisualElement.Q("sebesseg") as TextField;
		diameterField = mainDoc.rootVisualElement.Q("meret") as TextField;
		diameterField.RegisterCallback((ChangeEvent<string> e) =>
		{
			try
			{
				onDiameterChanged.Invoke(float.Parse(e.newValue));
			}
			catch (System.Exception exc)
			{
				Debug.LogException(exc);
			}
		});
		timeField = mainDoc.rootVisualElement.Q("ido") as TextField;

		//scrollview
		colours = mainDoc.rootVisualElement.Q("szinek") as ScrollView;
		colours.RegisterCallback<ClickEvent>(kolor);

		//renderwindow
		renderWindow = mainDoc.rootVisualElement.Q("rendererUSS") as VisualElement;

		//events
		dispenser.onButtonStateChanged.AddListener((bool b) =>
		{
			calculateButton.SetEnabled(b);
			calculateButton.style.color = new(b ? Color.white : Color.red);
		});

		gyroHandler.onGyroscopeStateChanged.AddListener((bool b) =>
		{
			if (!gyroButton.enabledInHierarchy) return;
			gyroButton.style.unityBackgroundImageTintColor = b ? Color.green : Color.white;
		});
		gyroHandler.onGyroButtonFlashed.AddListener(() =>
		{
			StartCoroutine(Utils.Flash(gyroButton, .3f, Color.red));
		});
	}
	private void Update()
	{
		//hunornak ha nincs adat
		litersLabel.text = Utils.FormatVolume(dispenser.fluidContainer.liters);
	}

	//errorLabel box state change based on event int
	void HandleError(int state)
	{
		switch (state)
		{
			case 0:
				errorLabel.style.display = DisplayStyle.None;
				errorLabel.text = "jó";
				renderWindow.style.display = DisplayStyle.Flex;
				break;
			case 1:
				renderWindow.style.display = DisplayStyle.None;
				errorLabel.style.display = DisplayStyle.Flex;
				errorLabel.text = "nem megfelelõ bemenet, kérlek csak számokat használj";
				break;
			case 2:
				renderWindow.style.display = DisplayStyle.None;
				errorLabel.style.display = DisplayStyle.Flex;
				errorLabel.text = "túl nagy a szám, hogy leszimuláljuk";
				break;
		}
	}

	//calc clicked, try to invoke inputfield data
	public void Calculate(ClickEvent evt)
	{
		Debug.Log("calc");
		try
		{
			onTimeChanged.Invoke(float.Parse(timeField.value));
			onSpeedChanged.Invoke(float.Parse(speedField.value));
			onDiameterChanged.Invoke(float.Parse(diameterField.value));
		}
		catch (System.Exception)
		{
			onErrorStateChanged.Invoke(1);
			throw;
		}
		onErrorStateChanged.Invoke(0);
		//call simulation func
		onCalculateButtonPressed.Invoke();
		//calculateButton.style.backgroundColor = Color.red;
	}

	//setting on or off
	public void ToggleSettings(ClickEvent evt)
	{
		Debug.Log("ToggleSettings");
		if (settingsMenu.activeSelf == false)
		{
			settingsMenu.SetActive(true);
			Debug.Log("be");
		}
		else { settingsMenu.SetActive(false); Debug.Log("ki"); }
	}

	//kolor test
	public void kolor(ClickEvent evt)
	{
		//change scrollview children background color 
		foreach (var child in colours.Children())
		{
			float a = Random.Range(0f, 1f);
			float b = Random.Range(0f, 1f);
			float c = Random.Range(0f, 1f);
			float d = Random.Range(0f, 1f);
			child.style.backgroundColor = new Color(a, b, c, d);
			var children = child as Label;
			children.text = a.ToString() + " " + b.ToString() + " " + c.ToString() + " " + d.ToString() + " ";
		}
	}
}
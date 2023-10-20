using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections.Generic;

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

	//define base colors
	public List<Color> colorList = new();
	public List<string> colorName = new();
	public UIDocument mainDoc;

	//ui elements
	[HideInInspector] public Button calculateButton, gyroButton;
	Button settingsButton;
	Label errorLabel, litersLabel;
	Label colorLabel, plusColorLabel;
	FloatField timeField, speedField, diameterField;
	ScrollView colours;
	VisualElement renderWindow;

	Coroutine flashCoroutine = null;

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
			gyroButton.SetEnabled(false);
		}

		settingsButton = mainDoc.rootVisualElement.Q("settings") as Button;
		settingsButton.RegisterCallback<ClickEvent>(ToggleSettings);

		//text
		litersLabel = mainDoc.rootVisualElement.Q("liters") as Label;
		errorLabel = mainDoc.rootVisualElement.Q("errorLabel") as Label;

		//inputfield
		speedField = mainDoc.rootVisualElement.Q("speed") as FloatField;
		diameterField = mainDoc.rootVisualElement.Q("diameter") as FloatField;
		diameterField.RegisterCallback((ChangeEvent<float> e) => onDiameterChanged.Invoke(e.newValue));
		timeField = mainDoc.rootVisualElement.Q("time") as FloatField;

		//scrollview
		colours = mainDoc.rootVisualElement.Q("szinek") as ScrollView;
        var @in = 0;
        foreach (var n in colorList)
		{
            colorLabel = new Label();
            colorLabel.AddToClassList("color");
			colorLabel.AddToClassList("text-outline");
			colorLabel.name= n.ToString();
			colorLabel.style.backgroundColor = n;
			colorLabel.text = colorName[@in];
			colorLabel.RegisterCallback<ClickEvent>((_) =>
			{
				dispenser.ital = n;
				calculateButton.style.backgroundColor = n;

			});
			colours.Add(colorLabel);

			//Label llaasd = mainDoc.rootVisualElement.Q(n.ToString) as Label;
			//llaasd.RegisterCallback<ClickEvent>();
			@in++;
		}

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
			if (flashCoroutine != null) StopCoroutine(flashCoroutine);
			flashCoroutine = StartCoroutine(Utils.Flash(gyroButton, .3f, Color.red));
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
		//Debug.Log("calc");
		try
		{
			onTimeChanged.Invoke(timeField.value);
			onSpeedChanged.Invoke(speedField.value);
			onDiameterChanged.Invoke(diameterField.value);
		}
		catch (System.Exception)
		{
			onErrorStateChanged.Invoke(1);
			throw;
		}
		onErrorStateChanged.Invoke(0);
		onCalculateButtonPressed.Invoke();
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
}
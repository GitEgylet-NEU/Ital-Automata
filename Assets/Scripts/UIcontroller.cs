using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class UIcontroller : MonoBehaviour
{
	public Dispenser dispenser;
	public GyroscopeHandler gyroHandler;
	public GameObject settingsMenu;
	UIDocument mainDoc;

	//ui elements
	Button calculateButton, settingsButton;
	Label errorLabel, litersLabel;
	TextField timeField, speedField, diameterField;
	ScrollView colours;
	VisualElement renderWindow;

	//events
	public UnityEvent<int> errorState;
	public UnityEvent<float> speedChanged, timeChanged, diameterChanged;
	void Start()
	{
		errorState.AddListener(HandleError);

		mainDoc = GetComponent<UIDocument>();

		//button
		calculateButton = mainDoc.rootVisualElement.Q("calculate") as Button;
		calculateButton.RegisterCallback<ClickEvent>(Calculate);

		settingsButton = mainDoc.rootVisualElement.Q("settings") as Button;
		settingsButton.RegisterCallback<ClickEvent>(ToggleSettings);

		//text
		litersLabel = mainDoc.rootVisualElement.Q("liters") as Label;
		errorLabel = mainDoc.rootVisualElement.Q("errorLabel") as Label;

		//inpufield
		speedField = mainDoc.rootVisualElement.Q("sebesseg") as TextField;
		diameterField = mainDoc.rootVisualElement.Q("meret") as TextField;
		timeField = mainDoc.rootVisualElement.Q("ido") as TextField;

		//scrollview
		colours = mainDoc.rootVisualElement.Q("szinek") as ScrollView;
		colours.RegisterCallback<ClickEvent>(kolor);

		//renderwindow
		renderWindow = mainDoc.rootVisualElement.Q("rendererUSS") as VisualElement;
	}
	private void Update()
	{
		//hunornak ha nincs adat
		//uncomment merge után
		litersLabel.text = Utils.FormatVolume(dispenser.fluidContainer.liters);
	}

	//errorLabel box state change based on event int
	public void HandleError(int state)
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
			timeChanged.Invoke(float.Parse(timeField.value));
			speedChanged.Invoke(float.Parse(speedField.value));
			diameterChanged.Invoke(float.Parse(diameterField.value));
		}
		catch (System.Exception)
		{
			errorState.Invoke(1);
			throw;
		}
		errorState.Invoke(0);
		//call simulation func
		calculateButton.style.backgroundColor = Color.red;
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
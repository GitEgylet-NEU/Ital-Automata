using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Dispenser : MonoBehaviour
{
	public static Dispenser instance;

	float atmero = 5f, sebesseg, ido; //cm, cm, s - kimenetben dm-nek kell lennie (1u=1dm)
	public Color ital = Utils.water;
	Transform waterFlow, spawnPoint;

	public FluidContainer fluidContainer;
	Coroutine dispenserCoroutine = null;

	/// <summary>Calls when the dispense button can be enabled. (true = can be set as interactable)</summary>
	public UnityEvent<bool> onButtonStateChanged = new();

	private void Awake()
	{
		instance = this;

		//TODO: call on change of átmérõ: transform.localScale = new Vector2((atmero + 1f) / 10f, 1);
		//TODO: listen to parameters (floats, Color)
		spawnPoint = transform.GetChild(0);
		waterFlow = spawnPoint.GetChild(0);
		waterFlow.gameObject.SetActive(false);

		//events
		UIcontroller.instance.onDiameterChanged.AddListener((float f) => atmero = f);
		UIcontroller.instance.onSpeedChanged.AddListener((float f) => sebesseg = f);
		UIcontroller.instance.onTimeChanged.AddListener((float f) => ido = f);
		UIcontroller.instance.onCalculateButtonPressed.AddListener(DispenseButton);
	}

	private void Update()
	{
		transform.localScale = new Vector2(atmero/10f, transform.localScale.y);
	}

	/// <summary>Call when the dispenser button has been pressed.</summary>
	public void DispenseButton()
	{
		if (dispenserCoroutine != null) return;

//		if (GyroscopeHandler.instance.gyroEnabled)
//		{
//#if UNITY_ANDROID && !UNITY_EDITOR //run only on android devices
//			Utils.ShowAndroidToastMessage("The gyroscope needs to be disabled!");
//#endif

//			//StartCoroutine(Utils.FlashImage(GyroscopeHandler.instance.gyroButton.image, .3f, Color.red));
//			GyroscopeHandler.instance.gyroscopeButtonFlashed.Invoke(); //tell UI to flash button
//			return;
//		}

		float flow = Calculator.CalculateFlow(atmero / 2f, sebesseg) / 1000f; // litersLabel/s
		waterFlow.localScale = new Vector2(atmero / 10f, spawnPoint.transform.position.y - (fluidContainer.transform.position.y - fluidContainer.height / 2f)) / (Vector2)transform.localScale;
		waterFlow.localPosition = new Vector2(0, -waterFlow.localScale.y / 2f);
		waterFlow.gameObject.SetActive(true);
		waterFlow.GetComponent<SpriteRenderer>().color = ital;

		dispenserCoroutine = StartCoroutine(DispenseAmount(flow, ido, ital));

		onButtonStateChanged.Invoke(false);
	}

	public IEnumerator DispenseAmount(float flow, float time, Color color)
	{
		float t = 0;
		float original = fluidContainer.liters;
		while (t <= time)
		{
			fluidContainer.AddFluid(flow * Time.deltaTime, color);
			//fluidContainer.litersLabel += flow * Time.deltaTime;
			t += Time.deltaTime;
			yield return null;
		}
		float diff = (original + time * flow) - fluidContainer.liters;
		if (diff > 0f) fluidContainer.AddFluid(diff, color);
		else fluidContainer.liters = original + time*flow;
		//fluidContainer.litersLabel = original + timeField * flow;
		waterFlow.gameObject.SetActive(false);
		dispenserCoroutine = null;

		onButtonStateChanged.Invoke(true);
	}
}
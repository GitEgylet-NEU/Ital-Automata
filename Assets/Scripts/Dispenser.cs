using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dispenser : MonoBehaviour
{
	float atmero, sebesseg, ido; //cm, cm, s - kimenetben dm-nek kell lennie (1u=1dm)
	Transform waterFlow, spawnPoint;

	[SerializeField] FluidContainer fluidContainer;
	Coroutine dispenserCoroutine;

	/// <summary>Calls when the state of the dispense button can be enabled. (true = can be set as interactable)</summary>
	public UnityEvent<bool> dispenseButtonCanBeEnabled = new();

	private void Awake()
	{
		//TODO: call on change of átmérõ: transform.localScale = new Vector2((atmero + 1f) / 10f, 1);
		//TODO: listen to parameters
		spawnPoint = transform.GetChild(0);
		waterFlow = spawnPoint.GetChild(0);
		waterFlow.gameObject.SetActive(false);
	}

	/// <summary>Call when the dispenser button has been pressed.</summary>
	public void DispenseButton()
	{
		if (dispenserCoroutine != null) return;

		if (GyroscopeHandler.instance.gyroEnabled)
		{
#if UNITY_ANDROID && !UNITY_EDITOR //run only on android devices
			Utils.ShowAndroidToastMessage("The gyroscope needs to be disabled!");
#endif

			//StartCoroutine(Utils.FlashImage(GyroscopeHandler.instance.gyroButton.image, .3f, Color.red));
			GyroscopeHandler.instance.gyroscopeButtonFlashed.Invoke(); //tell UI to flash button
			return;
		}

		float flow = Calculator.CalculateFlow(atmero / 2f, sebesseg) / 1000f; // liters/s
		waterFlow.localScale = new Vector2(atmero / 10f, spawnPoint.transform.position.y - (fluidContainer.transform.position.y - fluidContainer.height / 2f));
		waterFlow.localPosition = new Vector2(0, -waterFlow.localScale.y / 2f);
		waterFlow.gameObject.SetActive(true);

		StopCoroutine(dispenserCoroutine);
		dispenserCoroutine = StartCoroutine(DispenseAmount(flow, ido));

		dispenseButtonCanBeEnabled.Invoke(false);
	}

	IEnumerator DispenseAmount(float flow, float time)
	{
		float t = 0;
		float original = fluidContainer.liters;
		while (t <= time)
		{
			fluidContainer.liters += flow * Time.deltaTime;
			t += Time.deltaTime;
			yield return null;
		}
		fluidContainer.liters = original + time * flow;
		waterFlow.gameObject.SetActive(false);
		dispenserCoroutine = null;

		dispenseButtonCanBeEnabled.Invoke(true);
	}
}
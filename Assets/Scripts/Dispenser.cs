using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dispenser : MonoBehaviour
{
	[SerializeField] TMP_InputField atmeroInput, sebessegInput, idoInput;
	[SerializeField] TextMeshProUGUI quantityText;
	[SerializeField] Button dispenseButton;
	float atmero, sebesseg, ido; //cm, cm, s - kimenetben dm-nek kell lennie (1u=1dm)
	Transform waterFlow, spawnPoint;

	[SerializeField] FluidContainer fluidContainer;
	Coroutine dispenserCoroutine;
	AndroidJavaObject toast;

	/// <summary>Calls when the state of the dispense button should be changed. (true = set as interactable)</summary>
	public UnityEvent<bool> dispenseButtonChanged;
	public UnityEvent dispensingStarted, dispensingFinished;

	private void Awake()
	{
		atmeroInput.onEndEdit.AddListener((string v) =>
		{
			atmero = float.Parse(v);
			transform.localScale = new Vector2((atmero + 1f) / 10f, 1);
		});
		sebessegInput.onEndEdit.AddListener((string v) => sebesseg = float.Parse(v));
		idoInput.onEndEdit.AddListener((string v) => ido = float.Parse(v));
		quantityText.text = string.Empty;
		//dispenseButton.onClick.AddListener(DispenseButton);
		spawnPoint = transform.GetChild(0);
		waterFlow = spawnPoint.GetChild(0);
		waterFlow.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (string.IsNullOrEmpty(atmeroInput.text) || string.IsNullOrEmpty(sebessegInput.text) || string.IsNullOrEmpty(idoInput.text) || dispenserCoroutine != null)
		{
			dispenseButton.interactable = false;
			dispenseButtonChanged.Invoke(false);
		}
		else
		{
			dispenseButton.interactable = true;
			dispenseButtonChanged.Invoke(true);
		}
	}

	/// <summary>
	/// Call when the dispenser button has been pressed.
	/// </summary>
	public void DispenseButton()
	{
		if (GyroscopeHandler.instance.gyroEnabled)
		{
#if UNITY_ANDROID && !UNITY_EDITOR //run only on android devices                               
			if (toast != null) toast.Call("cancel");
			toast = Utils.ShowAndroidToastMessage("The gyroscope needs to be disabled!");
#endif
			StartCoroutine(Utils.FlashImage(GyroscopeHandler.instance.gyroButton.image, .3f, Color.red));
			return;
		}

		float flow = QuantityCalculator.CalculateFlow(atmero / 2f, sebesseg) / 1000f; // liters/s
		float q = flow * ido;
		quantityText.text = $"{Utils.FormatVolume(q)}\nflow: {Utils.FormatVolume(flow)}/s";
		waterFlow.localScale = new Vector2(atmero / 10f, spawnPoint.transform.position.y - (fluidContainer.transform.position.y - fluidContainer.height / 2f));
		waterFlow.localPosition = new Vector2(0, -waterFlow.localScale.y / 2f);
		waterFlow.gameObject.SetActive(true);
		dispenserCoroutine = StartCoroutine(DispenseAmount(flow, ido));
		dispensingStarted.Invoke();
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
		quantityText.text = string.Empty;
		waterFlow.gameObject.SetActive(false);
		dispenserCoroutine = null;
		dispensingFinished.Invoke();
	}
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuantityCalculatorTest : MonoBehaviour
{
	[SerializeField] TMP_InputField atmeroInput, sebessegInput, idoInput;
	[SerializeField] TextMeshProUGUI outputText;
	RectTransform rt;
	VerticalLayoutGroup verticalLayoutGroup;

	float atmero = 0, sebesseg = 0, ido = 0;

	void Start()
	{
		atmeroInput.text = string.Empty;
		sebessegInput.text = string.Empty;
		idoInput.text = string.Empty;
		outputText.text = string.Empty;

		rt = GetComponent<RectTransform>();
		verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
		
	}

	private void Update()
	{
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, verticalLayoutGroup.preferredHeight);
	}

	public void Szamitas()
	{
		bool folytat = true;
		try
		{
			atmero = float.Parse(atmeroInput.text);
			if (atmero <= 0)
			{
				//atmeroInput.setError("Adjon meg egy pozitív valós számot!");
				folytat = false;
			}
		}
		catch (Exception e)
		{
			//atmeroInput.setError("A szám formátuma helytelen!");
			Debug.LogException(e);
			folytat = false;
		}
		try
		{
			sebesseg = float.Parse(sebessegInput.text);
			if (sebesseg <= 0)
			{
				//sebessegInput.setError("Adjon meg egy pozitív valós számot!");
				folytat = false;
			}
		}
		catch (Exception e)
		{
			//sebessegInput.setError("A szám formátuma helytelen!");
			Debug.LogException(e);
			folytat = false;
		}
		try
		{
			ido = float.Parse(idoInput.text);
			if (ido <= 0)
			{
				//idoInput.setError("Adjon meg egy pozitív valós számot!");
				folytat = false;
			}
		}
		catch (Exception e)
		{
			//idoInput.setError("A szám formátuma helytelen!");
			Debug.LogException(e);
			folytat = false;
		}
		outputText.text = string.Empty;
		if (!folytat) return;

		float quantity = QuantityCalculator.CalculateQuantity(atmero / 2f, sebesseg, ido);
		outputText.text = Utils.FormatVolume(quantity / 1000f);
	}

	
}
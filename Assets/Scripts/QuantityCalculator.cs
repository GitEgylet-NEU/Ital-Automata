using System;
using TMPro;
using UnityEngine;

public class QuantityCalculator : MonoBehaviour
{
	[SerializeField] TMP_InputField atmeroInput, sebessegInput, idoInput;
	[SerializeField] TextMeshProUGUI outputText;

	float atmero = 0, sebesseg = 0, ido = 0;

	void Start()
	{
		atmeroInput.text = string.Empty;
		sebessegInput.text = string.Empty;
		idoInput.text = string.Empty;
		outputText.text = string.Empty;
	}

	public void Szamitas()
	{
		bool folytat = true;
		try
		{
			atmero = float.Parse(atmeroInput.text);
			if (atmero <= 0)
			{
				//atmeroInput.setError("Adjon meg egy pozit�v val�s sz�mot!");
				folytat = false;
			}
		}
		catch (Exception e)
		{
			//atmeroInput.setError("A sz�m form�tuma helytelen!");
			Debug.LogException(e);
			folytat = false;
		}
		try
		{
			sebesseg = float.Parse(sebessegInput.text);
			if (sebesseg <= 0)
			{
				//sebessegInput.setError("Adjon meg egy pozit�v val�s sz�mot!");
				folytat = false;
			}
		}
		catch (Exception e)
		{
			//sebessegInput.setError("A sz�m form�tuma helytelen!");
			Debug.LogException(e);
			folytat = false;
		}
		try
		{
			ido = float.Parse(idoInput.text);
			if (ido <= 0)
			{
				//idoInput.setError("Adjon meg egy pozit�v val�s sz�mot!");
				folytat = false;
			}
		}
		catch (Exception e)
		{
			//idoInput.setError("A sz�m form�tuma helytelen!");
			Debug.LogException(e);
			folytat = false;
		}
		outputText.text = string.Empty;
		if (!folytat) return;

		float quantity = CalculateQuantity(atmero / 2f, sebesseg, ido);
		outputText.text = UnitConverter.FormatVolume(quantity / 1000f);
	}

	/// <summary>
	/// radius and velocity must be in the same measurement! (cm, cm/s)
	/// I=v*A (t�rfogat �raml�sa = �raml�si sebess�g * mer�leges keresztmetszet)
	/// </summary>
	float CalculateFlow(float radius, float velocity)
	{
		float area = Mathf.PI * radius * radius; //cm^2
		return area * velocity; //(cm^3)/s
	}
	float CalculateQuantity(float radius, float velocity, float time)
	{
		float flow = CalculateFlow(radius, velocity);
		return flow * time; //ml=cm^3
	}
}
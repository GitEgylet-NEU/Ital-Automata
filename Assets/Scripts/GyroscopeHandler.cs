using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GyroscopeHandler : MonoBehaviour
{
	[SerializeField] FluidContainer fluidContainer;
	public bool gyroEnabled;
	public float rotation;
	Button gyroButton;
	Image gyroImage;

	[SerializeField] string format = "0.##";
	[SerializeField] Transform reference, actual;
	[SerializeField] TextMeshProUGUI gyroText;

	private void Awake()
	{
		gyroButton = GetComponent<Button>();
		gyroImage = GetComponent<Image>();
		gyroButton.onClick.AddListener(() =>
		{
			gyroEnabled = !gyroEnabled;
			if (gyroEnabled)
			{
				reference.rotation = GyroToUnity(Input.gyro.attitude);
			}
		});

		if (SystemInfo.supportsGyroscope)
		{
			Input.gyro.enabled = true;
			gyroEnabled = true;
			reference.rotation = GyroToUnity(Input.gyro.attitude);
		}
		else
		{
			Debug.LogWarning("No gyroscope could be found on the device. Disabling gyroscope functions.");
			gyroEnabled = false;
			gyroButton.interactable = false;
			gyroImage.color = Color.yellow;
			enabled = false;
		}
	}

	private void Update()
	{
		if (gyroEnabled)
		{
			gyroImage.color = Color.green;

			actual.rotation = GyroToUnity(Input.gyro.attitude);
			float angle = Vector3.SignedAngle(reference.up, actual.up, -reference.forward);
			gyroText.text = $"{actual.rotation.eulerAngles} (raw: {Input.gyro.attitude.eulerAngles})\n{angle.ToString(format)}";
			rotation = Mathf.Lerp(rotation, angle, Time.deltaTime * 10f);
		}
		else
		{
			gyroImage.color = Color.red;
			rotation = 0f;
		}
		fluidContainer.rotation = rotation;
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.magenta;
	//	Gizmos.DrawLine(actual.position, actual.position + 5f*actual.forward);
	//}

	//https://docs.unity3d.com/ScriptReference/Gyroscope.html
	private Quaternion GyroToUnity(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}
}
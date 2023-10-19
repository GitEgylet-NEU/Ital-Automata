using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GyroscopeHandler : MonoBehaviour
{
	public static GyroscopeHandler instance;

	[SerializeField] FluidContainer fluidContainer;
	public bool gyroEnabled;
	public float rotation;
	[HideInInspector] public Button gyroButton;

	[SerializeField] Transform reference, actual;

	public UnityEvent<bool> gyroscopeButtonEnabled = new();
	public UnityEvent<Color> gyroscopeButtonColorChanged = new();
	/// <summary>Calls when the gyroscope button should be flashed.
	/// Listeners should run <see cref="Utils.FlashImage(Image, float, Color)"/> and pass the gyroscope button's image.</summary>
	public UnityEvent gyroscopeButtonFlashed = new();

	private void Awake()
	{
		instance = this;

		gyroButton = GetComponent<Button>();

		if (SystemInfo.supportsGyroscope)
		{
			Input.gyro.enabled = true;
			gyroEnabled = true;

			gyroscopeButtonEnabled.Invoke(true);
		}
		else
		{
			Debug.LogWarning("No gyroscope could be found on the device. Disabling gyroscope functions.");
#if UNITY_ANDROID && !UNITY_EDITOR
			Utils.ShowAndroidToastMessage("Run the app on a device that supports a gyroscope to fully enjoy the experience!");
#endif
			gyroEnabled = false;
			gyroButton.interactable = false;
			gyroButton.image.color = Color.yellow;

			gyroscopeButtonEnabled.Invoke(false);
			gyroscopeButtonColorChanged.Invoke(Color.yellow);

			enabled = false;
		}
		
		//TODO: listen to button press
	}

	private void Start()
	{
		if (gyroEnabled)
		{
			reference.rotation = GyroToUnity(Input.gyro.attitude);
		}
	}

	private void Update()
	{
		if (gyroEnabled)
		{
			gyroButton.image.color = Color.green;
			gyroscopeButtonColorChanged.Invoke(Color.green);

			actual.rotation = GyroToUnity(Input.gyro.attitude);
			Vector3 eulerRot = actual.localRotation.eulerAngles;
			actual.localRotation = Quaternion.Euler(0, 0, eulerRot.z);
			float angle = Vector3.SignedAngle(reference.up, actual.up, -reference.forward);
			rotation = Mathf.Lerp(rotation, angle, Time.deltaTime * 10f); //smoothing
		}
		else
		{
			gyroButton.image.color = Color.red;
			gyroscopeButtonColorChanged.Invoke(Color.red);
			rotation = 0f;
		}
		fluidContainer.rotation = rotation;
	}

	/// <summary>Call when the gyroscope button has been pressed.</summary>
	public void GyroscopeButton()
	{
		gyroEnabled = !gyroEnabled;
		if (gyroEnabled)
		{
			reference.rotation = GyroToUnity(Input.gyro.attitude);
		}
	}

	//source: https://docs.unity3d.com/ScriptReference/Gyroscope.html
	private Quaternion GyroToUnity(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}
}
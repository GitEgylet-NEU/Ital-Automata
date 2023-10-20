using UnityEngine;
using UnityEngine.Events;

public class GyroscopeHandler : MonoBehaviour
{
	public static GyroscopeHandler instance;

	[SerializeField] FluidContainer fluidContainer;
	public bool gyroEnabled { get; private set; }
	float rotation;

	[SerializeField] Transform reference, actual;

	/// <summary>Calls when the gyroscope has been enabled or disabled.</summary>
	public UnityEvent<bool> onGyroscopeStateChanged;
	/// <summary>Calls when the gyroscope button should be flashed. Listeners should run <see cref="Utils.Flash"/> and pass the gyroscope button.</summary>
	public UnityEvent onGyroButtonFlashed;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (SystemInfo.supportsGyroscope)
		{
			Input.gyro.enabled = true;
		}
		else
		{
			Debug.LogWarning("No gyroscope could be found on the device. Disabling gyroscope functions.");
#if UNITY_ANDROID && !UNITY_EDITOR
			Utils.ShowAndroidToastMessage("Run the app on a device that supports a gyroscope to fully enjoy the experience!");
#endif
			gyroEnabled = false;
			enabled = false;
		}

		UIcontroller.instance.onGyroButtonPressed.AddListener(GyroscopeButton);
	}

	private void Update()
	{
		if (gyroEnabled)
		{
			actual.rotation = GyroToUnity(Input.gyro.attitude);
			Vector3 eulerRot = actual.localRotation.eulerAngles;
			actual.localRotation = Quaternion.Euler(0, 0, eulerRot.z);
			float angle = Vector3.SignedAngle(reference.up, actual.up, -reference.forward);
			rotation = Mathf.Lerp(rotation, angle, Time.deltaTime * 10f); //smoothing
		}
		else
		{
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
		onGyroscopeStateChanged.Invoke(gyroEnabled);
	}

	//source: https://docs.unity3d.com/ScriptReference/Gyroscope.html
	private Quaternion GyroToUnity(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}
}
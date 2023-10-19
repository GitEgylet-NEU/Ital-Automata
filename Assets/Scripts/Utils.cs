using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
	public static string FormatVolume(float litres)
	{
		float millilitres = litres * 1000;
		if (millilitres - 1000f >= 0f)
		{
			return (millilitres / 1000f).ToString("0.##") + " l";
		}
		else if (millilitres - 100f >= 0f)
		{
			return (millilitres / 100f).ToString("0.##") + " dl";
		}
		return millilitres.ToString("0.##") + " ml";
	}

	public static float GetVectorInternalAngle(Vector3 a, Vector3 b, Vector3 c)
	{
		return Vector3.Angle(a - b, c - b);
	}

#if UNITY_ANDROID && !UNITY_EDITOR
	public static AndroidJavaObject ShowAndroidToastMessage(string message)
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		if (unityActivity != null)
		{
			AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
			AndroidJavaObject toastObject = null;
			unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
				toastObject.Call("show");
			}));
			return toastObject;
		}
		return null;
	}
#endif

	public static IEnumerator FlashImage(Image image, float time, Color color)
	{
		float t = 0;
		Color originalColor = image.color;

		while (t <= time / 2f)
		{
			image.color = Color.Lerp(originalColor, color, t / (time / 2f));
			t += Time.deltaTime;
			yield return null;
		}
		image.color = color;
		t = 0;
		while (t <= time / 2f)
		{
			image.color = Color.Lerp(color, originalColor, t / (time / 2f));
			t += Time.deltaTime;
			yield return null;
		}
		
		image.color = originalColor;
	}
}

public static class MeshExtensions
{
	//source: https://gamedev.stackexchange.com/a/165647 (CC BY-SA 4.0)
	//not used in project, only for debugging during development
	public static float CalculateSurfaceArea(this Mesh mesh)
	{
		var triangles = mesh.triangles;
		var vertices = mesh.vertices;
		float sum = 0f;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 corner = vertices[triangles[i]];
			Vector3 a = vertices[triangles[i + 1]] - corner;
			Vector3 b = vertices[triangles[i + 2]] - corner;
			sum += Vector3.Cross(a, b).magnitude;
		}
		return sum / 2f;
	}
}
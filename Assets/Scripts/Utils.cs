using UnityEngine;

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
}

public static class MeshExtensions
{
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
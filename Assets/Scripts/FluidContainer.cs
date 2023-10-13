using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class FluidContainer : MonoBehaviour
{
	[Min(0)] public float width = 1f;
	[Min(0)] public float height = 1f;
	[Min(0)] public float thickness = .1f;
	[Range(-180, 180)] public float rotation = 0f;
	float capacity; // litres

	[Min(0)] public float litres = 0f;

	[SerializeField] TextMeshProUGUI text;
	LineRenderer lineRenderer;

	MeshFilter fluidMeshFilter;
	MeshRenderer fluidMeshRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = true;
		lineRenderer.positionCount = 4;
		lineRenderer.useWorldSpace = false;

		fluidMeshFilter = GetComponentInChildren<MeshFilter>();
		fluidMeshRenderer = GetComponentInChildren<MeshRenderer>();
	}

	private void OnEnable()
	{
		Awake();
	}

	// Update is called once per frame
	void Update()
	{
		//container
		lineRenderer.SetPosition(0, new Vector2(-(width + thickness), height + thickness));
		lineRenderer.SetPosition(1, new Vector2(-(width + thickness), -(height + thickness)));
		lineRenderer.SetPosition(2, new Vector2(width + thickness, -(height + thickness)));
		lineRenderer.SetPosition(3, new Vector2(width + thickness, height + thickness));
		lineRenderer.startWidth = thickness;
		lineRenderer.endWidth = thickness;
		capacity = UnitConverter.MetricToLitre(UnitConverter.UnityToMetric(width * height));

		if (litres >= capacity)
		{
			litres = capacity;
		}


		float neededHeight = UnitConverter.MetricToUnity(UnitConverter.LitreToMetric(litres) / width);
		//text.text = neededHeight.ToString();

		float heightA = Mathf.Tan(rotation * Mathf.Deg2Rad) * (width / 2f);
		Debug.Log("heightA=" + heightA);
		float heightB = Mathf.Tan(-rotation * Mathf.Deg2Rad) * (width / 2f);

		List<Vector3> vertices = new()
		{
			new Vector2(-width, neededHeight*2 - heightA*2),
			new Vector2(-width, 0),
			new Vector2(width, 0),
			new Vector2(width, neededHeight*2 - heightB*2)
		};
		if (heightA > neededHeight) //point A out of lower bounds
		{
			vertices.RemoveAt(1);
			vertices[0] = new Vector2(-Mathf.Tan(Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight*2, 0);
		}
		if (heightB > neededHeight)
		{
			vertices.RemoveAt(2);
			vertices[2] = new Vector2(Mathf.Tan(-Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight * 2, 0);
		}
		fluidMeshFilter.sharedMesh = GenerateFluidMesh(vertices.ToArray());

		float a = vertices[0].y/2f;
		float b = (vertices.Last().x + 1f)/2f;
		float c = Mathf.Sqrt(a*a + b*b);
		Debug.Log($"a={a}; b={b}; c={c}");
		Debug.Log("Area: " + (a * b / 2f));

		text.text = UnitConverter.MetricToLitre(UnitConverter.UnityToMetric(fluidMeshFilter.sharedMesh.CalculateSurfaceArea()/4f)).ToString("0.##") + " l"; //current fluid area

		fluidMeshRenderer.transform.localPosition = new Vector2(0, -height);
		transform.rotation = Quaternion.Euler(0, 0, -rotation);
			

		//text
		if (capacity == 0 || thickness == 0)
		{
			text.enabled = false;
		}
		else
		{
			//text.text = UnitConverter.FormatVolume(litres) + " / " + UnitConverter.FormatVolume(capacity);
			text.rectTransform.position = new Vector2(transform.position.x, transform.position.y-(height / 2 + thickness) - .3f);
			text.rectTransform.sizeDelta = new Vector2(width + thickness, .3f);
			text.enabled = true;
		}
	}

	Mesh GenerateFluidMesh(Vector3[] vertices)
	{
		Mesh mesh = new();
		mesh.name = "Fluid";
		int[] triangles;
		Vector2[] uvs;
		if (vertices.Length > 3)
		{
			triangles = new int[]
			{
				0, 1, 2,
				0, 2, 3
			};
			uvs = new Vector2[]
			{
				new Vector2(0, 1),
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1)
			};
		}
		else
		{
			triangles = new int[]
			{
				0, 1, 2
			};
			uvs = new Vector2[]
			{
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1)
			};
		}
		mesh.vertices = vertices;
		mesh.triangles = triangles.Reverse().ToArray();
		mesh.uv = uvs;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		return mesh;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector2 point = transform.GetChild(0).TransformPoint(fluidMeshFilter.sharedMesh.vertices[0]);
		point = new(0, point.y);
		Gizmos.DrawLine(point + new Vector2(-1, 0), point + new Vector2(1, 0));
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
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class FluidContainer : MonoBehaviour
{
	[Min(0)] public float widthMeters = 1f;
	[Min(0)] public float heightMeters = 1f;
	[Min(0)] public float thicknessMeters = .1f;
	[Range(-180, 180)] public float rotation = 0f;
	float capacity; // litres

	float width, height, thickness;

	[Min(0)] public float litres = 0f;

	[SerializeField] TextMeshProUGUI text, alphaText, betaText;
	LineRenderer lineRenderer;

	MeshFilter fluidMeshFilter;
	MeshRenderer fluidMeshRenderer;

	Vector2 pA, pC;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = true;
		lineRenderer.positionCount = 4;
		lineRenderer.useWorldSpace = false;

		fluidMeshFilter = GetComponentInChildren<MeshFilter>();
		fluidMeshRenderer = GetComponentInChildren<MeshRenderer>();
	}

	private void Reset()
	{
		Awake();
	}

	// Update is called once per frame
	void Update()
	{
		width = UnitConverter.MetricToUnity(widthMeters);
		height = UnitConverter.MetricToUnity(heightMeters);
		thickness = UnitConverter.MetricToUnity(thicknessMeters);

		//container
		lineRenderer.SetPosition(0, new Vector2(-(width + thickness), height + thickness));
		lineRenderer.SetPosition(1, new Vector2(-(width + thickness), -(height + thickness)));
		lineRenderer.SetPosition(2, new Vector2(width + thickness, -(height + thickness)));
		lineRenderer.SetPosition(3, new Vector2(width + thickness, height + thickness));
		lineRenderer.startWidth = thickness;
		lineRenderer.endWidth = thickness;
		capacity = UnitConverter.MetricToLitre(UnitConverter.UnityToMetric(width * height));
		transform.rotation = Quaternion.Euler(0, 0, -rotation);

		if (litres >= capacity)
		{
			litres = capacity;
		}

		if (Mathf.Abs(rotation) >= 90f)
		{
			litres = 0f;
			fluidMeshRenderer.enabled = false;
			return;
		}
		else
		{
			fluidMeshRenderer.enabled = true;
		}

		float neededHeight = UnitConverter.MetricToUnity(UnitConverter.LitreToMetric(litres) / widthMeters);
		//text.text = neededHeight.ToString();

		float heightA = UnitConverter.MetricToUnity(Mathf.Tan(rotation * Mathf.Deg2Rad) * (widthMeters / 2f));
		float heightB = UnitConverter.MetricToUnity(Mathf.Tan(-rotation * Mathf.Deg2Rad) * (widthMeters / 2f));
		//Debug.Log("heightA=" + heightA);

		List<Vector3> vertices = new()
		{
			new Vector2(-width, neededHeight*2 - heightA*2),	//A
			new Vector2(-width, 0),								//B
			new Vector2(width, 0),								//C
			new Vector2(width, neededHeight*2 - heightB*2)		//D
		};


		int triangle = 0;

		//calculate triangle of point A (when point D doesn't reach lower right corner of container) for reference
		Vector2 C1 = new(Mathf.Tan(-Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight * 2, 0);
		float alpha1 = GetVectorInternalAngle(vertices[1], vertices[0], C1);	//B, A, C
		float beta1 = GetVectorInternalAngle(vertices[1], C1, vertices[0]);		//B, C, A
		float b1 = Mathf.Sqrt((2 * UnitConverter.LitreToMetric(litres) * Mathf.Sin(alpha1 * Mathf.Deg2Rad)) / Mathf.Sin(beta1 * Mathf.Deg2Rad));
		float a1 = (2 * UnitConverter.LitreToMetric(litres)) / b1;
		Vector2 A1 = (Vector2)vertices[1] + Vector2.up * UnitConverter.MetricToUnity(a1);
		C1 = (Vector2)vertices[1] + Vector2.right * UnitConverter.MetricToUnity(b1);
		if (rotation < 0f && transform.GetChild(0).TransformPoint(C1).y <= transform.TransformPoint(lineRenderer.GetPosition(2)).y && transform.GetChild(0).TransformPoint(A1).y > transform.TransformPoint(lineRenderer.GetPosition(1)).y)
		{
			Debug.Log("Triangle A should be used");
			triangle = 1;
		}

		//calculate triangle of point D (when point A doesn't reach lower right corner of container) for reference
		Vector2 B2 = new(-Mathf.Tan(Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight * 2, 0);
		float alpha2 = GetVectorInternalAngle(B2, vertices[3], vertices[2]);
		float beta2 = GetVectorInternalAngle(vertices[2], B2, vertices[3]);
		float b2 = Mathf.Sqrt(2 * UnitConverter.LitreToMetric(litres) * Mathf.Sin(alpha2 * Mathf.Deg2Rad) / Mathf.Sin(beta2 * Mathf.Deg2Rad));
		float a2 = 2 * UnitConverter.LitreToMetric(litres) / b2;
		B2 = (Vector2)vertices[2] + Vector2.left * UnitConverter.MetricToUnity(b2);
		Vector2 A2 = (Vector2)vertices[2] + Vector2.up * UnitConverter.MetricToUnity(a2);
		if (rotation > 0f && transform.GetChild(0).TransformPoint(B2).y <= transform.TransformPoint(lineRenderer.GetPosition(1)).y && transform.GetChild(0).TransformPoint(A2).y > transform.TransformPoint(lineRenderer.GetPosition(2)).y)
		{
			Debug.Log("Triangle B should be used");
			triangle = 2;
		}



		//if (heightA > neededHeight) //point A out of lower bounds
		//{
		//	vertices.RemoveAt(1);
		//	vertices[0] = new Vector2(-Mathf.Tan(Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight*2, 0);
		//}
		//if (heightB > neededHeight)
		//{
		//	vertices.RemoveAt(2);
		//	vertices[2] = new Vector2(Mathf.Tan(-Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight * 2, 0);
		//}

		fluidMeshRenderer.transform.localPosition = new Vector2(0, -height);
		

		float alpha = GetVectorInternalAngle(vertices[1], vertices[0], vertices.Last());
		float beta = GetVectorInternalAngle(vertices[1], vertices.Last(), vertices[0]);
		alphaText.text = "α=" + alpha.ToString("0.#") + '°';
		betaText.text = "β=" + beta.ToString("0.#") + '°';

		//new values
		float b = Mathf.Sqrt((2 * UnitConverter.LitreToMetric(litres) * Mathf.Sin(alpha * Mathf.Deg2Rad)) / Mathf.Sin(beta * Mathf.Deg2Rad));
		float a = (2 * UnitConverter.LitreToMetric(litres)) / b;
		Vector2 newA = Vector2.up * UnitConverter.MetricToUnity(a);
		Vector2 newC = Vector2.right * UnitConverter.MetricToUnity(b);
		//Debug.Log($"a={UnitConverter.MetricToUnity(a)}; b={UnitConverter.MetricToUnity(b)}");
		//Debug.Log($"A={newA}; C={newC}");
		float area = a * b / 2f;
		text.text = UnitConverter.MetricToLitre(area).ToString("0.##") + " l";

		//vertices[0] = (Vector2)vertices[1] + newA;
		//vertices[2] = (Vector2)vertices[1] + newC;
		//if (vertices.Count == 4) vertices.RemoveAt(1);

		switch (triangle)
		{
			case 0:
				Debug.Log("Quad should be used");
				vertices[0] = new Vector2(-width, neededHeight*2 - heightA*2);
				vertices[3] = new Vector2(width, neededHeight*2 - heightB*2);
				break;
			case 1:
				if (vertices.Count == 4) vertices.RemoveAt(2);
				vertices[0] = (Vector2)vertices[1] + Vector2.up * UnitConverter.MetricToUnity(a1);
				vertices[2] = (Vector2)vertices[1] + Vector2.right * UnitConverter.MetricToUnity(b2);
				break;
			case 2:
				if (vertices.Count == 4) vertices.RemoveAt(1);
				vertices[0] = (Vector2)vertices[1] + Vector2.left * UnitConverter.MetricToUnity(b2);
				vertices[2] = (Vector2)vertices[1] + Vector2.up * UnitConverter.MetricToUnity(a2);
				break;
			default:
				break;
		}

		
		text.text = UnitConverter.MetricToLitre(a2 * b2 / 2f).ToString("0.##") + " l";

		pA = vertices[0];
		pC = vertices[2];

		pA = B2;
		pC = A2;

		//generate and assign mesh
		fluidMeshFilter.sharedMesh = GenerateFluidMesh(vertices.ToArray());

		//text
		if (capacity == 0 || thickness == 0)
		{
			text.enabled = false;
		}
		else
		{
			//text.text = UnitConverter.FormatVolume(litres) + " / " + UnitConverter.FormatVolume(capacity);
			text.text = (fluidMeshFilter.sharedMesh.CalculateSurfaceArea()*10f).ToString("0.##");
			text.rectTransform.position = new Vector2(transform.position.x, transform.position.y-(height / 2 + thickness) - .3f - .25f);
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

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.GetChild(0).TransformPoint(pA), transform.GetChild(0).TransformPoint(pC));
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
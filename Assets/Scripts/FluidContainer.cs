using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class FluidContainer : MonoBehaviour
{
	// 1u = 1dm
	// 1l = 1dm^2
	[Min(0), Tooltip("Width of container in decimetres")] public float width = 1f;
	[Min(0), Tooltip("Height of container in decimetres")] public float height = 1f;
	[Min(0), Tooltip("Thickness of container in decimetres")] public float thickness = .1f;
	[Range(-180, 180)] public float rotation = 0f;
	float capacity; // liters

	[Min(0)] public float liters = 0f;

	[SerializeField] TextMeshProUGUI text, alphaText, betaText;
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

	private void Reset()
	{
		Awake();
	}

	// Update is called once per frame
	void Update()
	{
		//container
		lineRenderer.SetPosition(0, new Vector2(-(width / 2f + thickness / 2f), height / 2f + thickness / 2f));
		lineRenderer.SetPosition(1, new Vector2(-(width / 2f + thickness / 2f), -(height / 2f + thickness / 2f)));
		lineRenderer.SetPosition(2, new Vector2(width / 2f + thickness / 2f, -(height / 2f + thickness / 2f)));
		lineRenderer.SetPosition(3, new Vector2(width / 2f + thickness / 2f, height / 2f + thickness / 2f));
		lineRenderer.startWidth = thickness;
		lineRenderer.endWidth = thickness;
		capacity = width * height;

		Vector3[] containerPos = new Vector3[lineRenderer.positionCount];
		lineRenderer.GetPositions(containerPos);

		transform.rotation = Quaternion.Euler(0, 0, -rotation);
		fluidMeshRenderer.transform.localPosition = new Vector2(0, -height / 2);

		if (liters >= capacity)
		{
			liters = capacity;
		}

		if (Mathf.Abs(rotation) >= 90f)
		{
			liters = 0f;
		}

		//text
		if (capacity == 0 || thickness == 0)
		{
			text.enabled = false;
		}
		else
		{
			text.text = Utils.FormatVolume(liters) + " / " + Utils.FormatVolume(capacity);
			float lowestY = containerPos.Select(x => transform.TransformPoint(x).y).Min();
			text.rectTransform.position = new Vector2(transform.position.x, lowestY - .3f);
			text.rectTransform.sizeDelta = new Vector2(width + thickness * 2, .3f);
			text.enabled = true;
		}
		if (liters == 0f)
		{
			fluidMeshRenderer.enabled = false;
			return;
		}
		else
		{
			fluidMeshRenderer.enabled = true;
		}

		float neededHeight = liters / width;
		float heightA = Mathf.Tan(rotation * Mathf.Deg2Rad) * (width / 2f);
		float heightB = Mathf.Tan(-rotation * Mathf.Deg2Rad) * (width / 2f);

		List<Vector3> vertices = new()
		{
			new Vector2(-width/2, neededHeight - heightA), //A
			new Vector2(-width/2, 0), //B
			new Vector2(width/2, 0), //C
			new Vector2(width/2, neededHeight - heightB) //D
		};

		int triangle = 0;

		//calculate triangle of point A (when point D doesn't reach lower right corner of container) for reference
		Vector2 C1 = new(Mathf.Tan(-Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight, 0);
		float alpha1 = Utils.GetVectorInternalAngle(vertices[1], vertices[0], C1); //B, A, C
		float beta1 = Utils.GetVectorInternalAngle(vertices[1], C1, vertices[0]); //B, C, A
		float b1 = Mathf.Sqrt((2 * liters * Mathf.Sin(alpha1 * Mathf.Deg2Rad)) / Mathf.Sin(beta1 * Mathf.Deg2Rad));
		float a1 = (2 * liters) / b1;
		Vector2 A1 = (Vector2)vertices[1] + Vector2.up * a1;
		C1 = (Vector2)vertices[1] + Vector2.right * b1;
		if (rotation < 0f && transform.GetChild(0).TransformPoint(C1).y <= transform.TransformPoint(lineRenderer.GetPosition(2)).y && transform.GetChild(0).TransformPoint(A1).y > transform.TransformPoint(lineRenderer.GetPosition(1)).y)
		{
			Debug.Log("Triangle A should be used");
			triangle = 1;
		}

		//calculate triangle of point D (when point A doesn't reach lower right corner of container) for reference
		Vector2 B2 = new(-Mathf.Tan(Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight, 0);
		float alpha2 = Utils.GetVectorInternalAngle(B2, vertices[3], vertices[2]);
		float beta2 = Utils.GetVectorInternalAngle(vertices[2], B2, vertices[3]);
		float b2 = Mathf.Sqrt(2 * liters * Mathf.Sin(alpha2 * Mathf.Deg2Rad) / Mathf.Sin(beta2 * Mathf.Deg2Rad));
		float a2 = 2 * liters / b2;
		B2 = (Vector2)vertices[2] + Vector2.up * a2;
		Vector2 A2 = (Vector2)vertices[2] + Vector2.left * b2;
		if (rotation > 0f && transform.GetChild(0).TransformPoint(B2).y <= transform.TransformPoint(lineRenderer.GetPosition(1)).y && transform.GetChild(0).TransformPoint(A2).y > transform.TransformPoint(lineRenderer.GetPosition(2)).y)
		{
			Debug.Log("Triangle B should be used");
			triangle = 2;
		}

		switch (triangle)
		{
			case 0:
				Debug.Log("Quad should be used");
				alphaText.text = "α=" + Utils.GetVectorInternalAngle(vertices[1], vertices[0], vertices[3]).ToString("0.#") + '°';
				betaText.text = "β=" + Utils.GetVectorInternalAngle(vertices[1], vertices[0], vertices[3]).ToString("0.#") + '°';
				break;
			case 1:
				if (vertices.Count == 4) vertices.RemoveAt(2);
				vertices[0] = (Vector2)vertices[1] + Vector2.up * a1;
				vertices[2] = (Vector2)vertices[1] + Vector2.right * b1;
				alphaText.text = "α=" + alpha1.ToString("0.#") + '°';
				betaText.text = "β=" + beta1.ToString("0.#") + '°';
				break;
			case 2:
				if (vertices.Count == 4) vertices.RemoveAt(1);
				vertices[0] = (Vector2)vertices[1] + Vector2.left * b2;
				vertices[2] = (Vector2)vertices[1] + Vector2.up * a2;
				alphaText.text = "α=" + alpha2.ToString("0.#") + '°';
				betaText.text = "β=" + beta2.ToString("0.#") + '°';
				break;
		}

		//generate and assign mesh
		fluidMeshFilter.sharedMesh = GenerateFluidMesh(vertices.ToArray());


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
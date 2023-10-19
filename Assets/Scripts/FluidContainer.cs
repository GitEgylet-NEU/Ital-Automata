using System.Collections.Generic;
using System.Linq;
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

	[Min(0)] public float liters = 0f;
	public Color fluidColor = Utils.water;

	bool shouldSimulate = true;

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

		//TODO: listen to errors from UI -> shouldSimulate
	}
	private void Reset()
	{
		Awake();
	}

	/// <param name="amount">Amount of fluid in liters</param>
	public void AddFluid(float amount, Color color)
	{
		if (liters == 0f)
		{
			liters = amount;
			fluidColor = color;
			return;
		}
		//in theory, new color can be calculated by taking the weighted average of the two fluids' colors
		float averageR = (liters * fluidColor.r + amount * color.r) / (liters + amount);
		float averageG = (liters * fluidColor.g + amount * color.g) / (liters + amount);	
		float averageB = (liters * fluidColor.b + amount * color.b) / (liters + amount);
		float averageA = (liters * fluidColor.a + amount * color.a) / (liters + amount);
		fluidColor = new Color(averageR, averageG, averageB, averageA);
		//Debug.Log($"{liters} * {fluidColor.r} & {amount} * {color.r} | {averageR}"); //debug
		liters += amount;
	}

	void Update()
	{
		lineRenderer.enabled = shouldSimulate;
		fluidMeshRenderer.enabled = shouldSimulate;
		if (!shouldSimulate) return;

		//container
		lineRenderer.SetPosition(0, new Vector2(-(width / 2f + thickness / 2f), height / 2f + thickness / 2f));
		lineRenderer.SetPosition(1, new Vector2(-(width / 2f + thickness / 2f), -(height / 2f + thickness / 2f)));
		lineRenderer.SetPosition(2, new Vector2(width / 2f + thickness / 2f, -(height / 2f + thickness / 2f)));
		lineRenderer.SetPosition(3, new Vector2(width / 2f + thickness / 2f, height / 2f + thickness / 2f));
		lineRenderer.startWidth = thickness;
		lineRenderer.endWidth = thickness;
		Vector3[] containerPos = new Vector3[lineRenderer.positionCount];
		lineRenderer.GetPositions(containerPos);

		fluidMeshRenderer.transform.localPosition = new Vector2(0, -height / 2);
		if (Mathf.Abs(rotation) >= 90f) liters = 0f;

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.R))
		{
			StartCoroutine(FindObjectOfType<Dispenser>().DispenseAmount(.2f, 1f, Color.red));
		}
		else if (Input.GetKeyDown(KeyCode.G)) AddFluid(.05f, Color.green);
		else if (Input.GetKeyDown(KeyCode.B)) AddFluid(.05f, Color.blue);
#endif

		if (liters == 0f)
		{
			fluidMeshRenderer.enabled = false;
			fluidMeshFilter.sharedMesh = null;
			return;
		}
		else fluidMeshRenderer.enabled = true;

		fluidMeshRenderer.sharedMaterial.color = fluidColor;

		float neededHeight = liters / width;
		float heightA = Mathf.Tan(rotation * Mathf.Deg2Rad) * (width / 2f);
		float heightB = Mathf.Tan(-rotation * Mathf.Deg2Rad) * (width / 2f);
		List<Vector3> vertices = new()
		{
			new Vector2(-width/2, neededHeight - heightA),
			new Vector2(-width/2, 0),
			new Vector2(width/2, 0),
			new Vector2(width/2, neededHeight - heightB)
		};

		//triangle A
		Vector2 C1 = new(Mathf.Tan(-Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight, 0);
		float alpha1 = Utils.GetVectorInternalAngle(vertices[1], vertices[0], C1);
		float beta1 = Utils.GetVectorInternalAngle(vertices[1], C1, vertices[0]);
		float b1 = Mathf.Sqrt((2 * liters * Mathf.Sin(alpha1 * Mathf.Deg2Rad)) / Mathf.Sin(beta1 * Mathf.Deg2Rad));
		float a1 = (2 * liters) / b1;
		Vector2 A1 = (Vector2)vertices[1] + Vector2.up * a1;
		C1 = (Vector2)vertices[1] + Vector2.right * b1;
		if (rotation < 0f
			&& transform.GetChild(0).TransformPoint(C1).y <= transform.TransformPoint(containerPos[2]).y + thickness / 2f
			&& transform.GetChild(0).TransformPoint(C1).x <= transform.TransformPoint(containerPos[2]).x - thickness / 2f
			&& transform.GetChild(0).TransformPoint(A1).y > transform.TransformPoint(containerPos[1]).y + thickness / 2f)
		{
			//Debug.Log("Triangle A should be used");
			if (A1.y > height)
			{
				float diff = A1.y - (height);
				A1 = new Vector2(A1.x, A1.y - diff);
				a1 = A1.y;
				b1 = Mathf.Tan(alpha1 * Mathf.Deg2Rad) * a1;
				liters = b1 * a1 / 2f;
				neededHeight = liters / width;
			}

			if (vertices.Count == 4) vertices.RemoveAt(2);
			vertices[0] = (Vector2)vertices[1] + Vector2.up * a1;
			vertices[2] = (Vector2)vertices[1] + Vector2.right * b1;

			goto rendering;
		}

		//triangle B
		Vector2 B2 = new(-Mathf.Tan(Mathf.Abs(90f - rotation) * Mathf.Deg2Rad) * neededHeight, 0);
		float alpha2 = Utils.GetVectorInternalAngle(B2, vertices[3], vertices[2]);
		float beta2 = Utils.GetVectorInternalAngle(vertices[2], B2, vertices[3]);
		float b2 = Mathf.Sqrt(2 * liters * Mathf.Sin(alpha2 * Mathf.Deg2Rad) / Mathf.Sin(beta2 * Mathf.Deg2Rad));
		float a2 = 2 * liters / b2;
		B2 = (Vector2)vertices[2] + Vector2.up * a2;
		Vector2 A2 = (Vector2)vertices[2] + Vector2.left * b2;
		if (rotation > 0f
			&& transform.GetChild(0).TransformPoint(A2).y <= transform.TransformPoint(containerPos[1]).y + thickness / 2f
			&& transform.GetChild(0).TransformPoint(A2).x >= transform.TransformPoint(containerPos[1]).x + thickness / 2f
			&& transform.GetChild(0).TransformPoint(B2).y > transform.TransformPoint(containerPos[2]).y + thickness / 2f)
		{
			//Debug.Log("Triangle B should be used");
			if (B2.y > height)
			{
				float diff = B2.y - (height);
				B2 = new Vector2(B2.x, B2.y - diff);
				a2 = B2.y;
				b2 = Mathf.Tan(alpha2 * Mathf.Deg2Rad) * a2;
				liters = b2 * a2 / 2f;
				neededHeight = liters / width;
			}

			if (vertices.Count == 4) vertices.RemoveAt(1);
			vertices[0] = (Vector2)vertices[1] + Vector2.left * b2;
			vertices[2] = (Vector2)vertices[1] + Vector2.up * a2;

			goto rendering;
		}

		if (vertices.Count == 4)
		{
			//Debug.Log("Quad should be used");
			float diffA = vertices[0].y - (height);
			float diffB = vertices[3].y - (height);
			if (diffA > 0f) //pour at point A
			{
				neededHeight = Mathf.Max(neededHeight - diffA, 0f);
			}
			else if (diffB > 0f) //pour at point B
			{
				neededHeight = Mathf.Max(neededHeight - diffB, 0f);
			}
			liters = neededHeight * width;
			vertices[0] = new Vector2(-width / 2, neededHeight - heightA);
			vertices[3] = new Vector2(width / 2, neededHeight - heightB);

			goto rendering;
		}

	rendering:
		//generate and assign mesh
		fluidMeshFilter.sharedMesh = GenerateFluidMesh(vertices.ToArray());
	}

	Mesh GenerateFluidMesh(Vector3[] vertices)
	{
		if (vertices.Length < 3) return null;
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
}
using System.Linq;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class FluidContainer : MonoBehaviour
{
	[Min(0)] public float width = 1f;
	[Min(0)] public float height = 1f;
	[Min(0)] public float thickness = .1f;
	[Range(0, 360)] public float rotation = 0f;
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
		fluidMeshFilter.sharedMesh = new Mesh();
		fluidMeshFilter.sharedMesh.name = "Fluid";
	}

	private void OnEnable()
	{
		Awake();
	}

	// Update is called once per frame
	void Update()
	{
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

		Vector3[] vertices = new Vector3[]
		{
			new Vector2(-width, neededHeight*2),
			new Vector2(-width, 0),
			new Vector2(width, 0),
			new Vector2(width, neededHeight*2)
		};
		int[] triangles = new int[]
		{
			0, 1, 2,
			0, 2, 3
		};
		Vector2[] uvs = new Vector2[]
		{
			new Vector2(0, 1),
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 1)
		};
		fluidMeshFilter.sharedMesh.vertices = vertices;
		fluidMeshFilter.sharedMesh.triangles = triangles.Reverse().ToArray();
		fluidMeshFilter.sharedMesh.uv = uvs;
		fluidMeshFilter.sharedMesh.RecalculateBounds();
		fluidMeshFilter.sharedMesh.RecalculateNormals();
		fluidMeshRenderer.transform.localPosition = new Vector2(0, -height);

		if (capacity == 0 || thickness == 0)
		{
			text.enabled = false;
		}
		else
		{
			text.text = UnitConverter.FormatVolume(litres) + " / " + UnitConverter.FormatVolume(capacity);
			text.rectTransform.position = new Vector2(0, -(height / 2 + thickness) - .3f);
			text.rectTransform.sizeDelta = new Vector2(width + thickness, .3f);
			text.enabled = true;
		}
	}
}
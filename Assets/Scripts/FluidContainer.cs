using TMPro;
using UnityEngine;

[ExecuteAlways]
public class FluidContainer : MonoBehaviour
{
	[Min(0)] public float width = 1f;
	[Min(0)] public float height = 1f;
	[Min(0)] public float thickness = .1f;
	float capacity; // litres

	[Min(0)] public float litres = 0f;

	[SerializeField] TextMeshProUGUI text;
	[SerializeField] Transform fluid;
	LineRenderer lineRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = true;
		lineRenderer.positionCount = 4;
		lineRenderer.useWorldSpace = false;
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
		fluid.position = new Vector2(transform.position.x, transform.position.y - height / 2 + (neededHeight / 2));
		fluid.localScale = new Vector2(width, neededHeight);

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
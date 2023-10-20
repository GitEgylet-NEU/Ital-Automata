using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowSimulation : MonoBehaviour
{
	public UIDocument mainrender;
	RenderTexture renderTexture;
	VisualElement rendererui;
	private void Start()
	{
		Dispenser.instance.onButtonStateChanged.Invoke(false); //disable button
		rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
		StartCoroutine(DelayedStart(0.25f));
	}

	IEnumerator DelayedStart(float t)
	{
		yield return new WaitForSeconds(t);

		renderTexture = new(Convert.ToInt32(rendererui.contentRect.width), Convert.ToInt32(rendererui.contentRect.height), 2);
		renderTexture.name = "SimulationRenderTexture";

		GetComponent<Camera>().targetTexture = renderTexture;
		rendererui.style.backgroundImage = Background.FromRenderTexture(renderTexture);
		rendererui.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(BackgroundSizeType.Contain));

		Dispenser.instance.onButtonStateChanged.Invoke(true); //re-enable button
	}
}
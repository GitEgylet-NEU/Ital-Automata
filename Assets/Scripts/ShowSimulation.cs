using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowSimulation : MonoBehaviour
{
	public UIDocument mainrender;
	RenderTexture renderTexture;
	VisualElement rendererui;
	Button calcClick;
	private void Start()
	{
		calcClick = mainrender.rootVisualElement.Q("calculate") as Button;
		calcClick.SetEnabled(false);
		rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
		Debug.Log("startkor: " + rendererui.contentRect.width);
		StartCoroutine(Delay(0.7f));
	}

	IEnumerator Delay(float t)
	{
		yield return new WaitForSeconds(t);

		rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
		Debug.Log("sz�lte:" + rendererui.contentRect.width);
		renderTexture = new(Convert.ToInt32(rendererui.contentRect.width), Convert.ToInt32(rendererui.contentRect.height), 2);
		GetComponent<Camera>().targetTexture = renderTexture;
		Debug.Log("rendsize:" + renderTexture.width);
		var bg = Background.FromRenderTexture(renderTexture);
		rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
		rendererui.style.backgroundImage = bg;
		rendererui.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(BackgroundSizeType.Contain));
		calcClick.SetEnabled(true);
	}
}
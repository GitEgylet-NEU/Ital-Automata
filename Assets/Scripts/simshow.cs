using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class simshow : MonoBehaviour
{
    public Camera simCam;
    public UIDocument mainrender;
    public RenderTexture rendtexture;
    VisualElement rendererui;
    private void Start()
    {
        rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
        Debug.Log("startkor: " + rendererui.contentRect.width);
        StartCoroutine(Delay(1.5f));
        
    }
    private void Update()
    {
        //if (run) 
        //{
        //    var bg = Background.FromRenderTexture(rendtexture);
        //    rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
        //    rendererui.style.backgroundImage = bg;
        //}
        //  Debug.Log(renderer.contentRect.width);
    }
    IEnumerator Delay(float t)
    {
        yield return new WaitForSeconds(t);
        rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
        Debug.Log("szélte:" + rendererui.contentRect.width);
        rendtexture = new(Convert.ToInt32(rendererui.contentRect.width), Convert.ToInt32(rendererui.contentRect.height), 2);
        Debug.Log("rendsize:" + rendtexture.width);
        simCam.targetTexture = rendtexture;
        var bg = Background.FromRenderTexture(rendtexture);
        rendererui = mainrender.rootVisualElement.Q("rendererUSS") as VisualElement;
        rendererui.style.backgroundImage = bg;
        rendererui.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(BackgroundSizeType.Contain));
    }
}
